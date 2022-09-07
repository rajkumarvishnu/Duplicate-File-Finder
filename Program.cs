﻿// For Directory.GetFiles and Directory.GetDirectories
// For File.Exists, Directory.Exists
using System;
using System.IO;
using System.Collections;

public class FileInformation
{
    public string path { get; set; }
    public long size { get; set; }
    public string filename { get; set; }
    public DateTime creationtime { get; set; }
}

public static class Constants
{
    public const int one_mb = 1048576;
    public const long filter_size = one_mb * 5;
    public const long filter_size_in_MBs = filter_size / one_mb;
}

public class Duplicate
{
    public string filename { get; set; }
    public int count { get; set; }
    public List<string> paths { get; set; }
    public long size { get; set; }
}

public class RecursiveFileProcessor
{
    public static List<FileInformation> FI = new List<FileInformation>();
    public static List<Duplicate> Dups = new List<Duplicate>();
    public static void Main(string[] args)
    {
        Console.WriteLine("Starting...");

        List<string> paths = new List<string>();
        paths.Add("/Users/vishnurajkumar/Pictures/Archive");
        foreach (var item in args)
        {
            Console.WriteLine(item);
        }

        foreach (string path in paths)
        {
            if (File.Exists(path))
            {
                // This path is a file
                ProcessFile(path);
            }
            else if (Directory.Exists(path))
            {
                // This path is a directory
                ProcessDirectory(path);
            }
            else
            {
                Console.WriteLine("{0} is not a valid file or directory.", path);
            }
        }
        /*
                Console.WriteLine("_____________________________________________");
                
                foreach (var item in FI)
                {
                    Console.WriteLine(item.path + "  ---  " + item.size);
                }
        */
        FindDuplicates();
    }

    public static void FindDuplicates()
    {
        foreach (var item in FI)
        {
            //  Console.WriteLine("Looking for duplicates - " + item.filename);
            var potentialduplicates = FI.FindAll(
                x =>
                    x.filename == item.filename
                    && item.size == x.size
                    && item.creationtime == x.creationtime
            );
            if (potentialduplicates.Count > 1)
            {
                var dup = Dups.Find(
                    x => x.filename == potentialduplicates.FirstOrDefault().filename
                );
                if (dup == null)
                {
                    Duplicate d = new Duplicate();
                    d.paths = new List<string>();
                    foreach (var dupitem in potentialduplicates)
                    {
                        d.paths.Add(dupitem.path);
                    }
                    d.filename = potentialduplicates.FirstOrDefault().filename;
                    d.count = potentialduplicates.Count;
                    d.size = d.size + potentialduplicates.FirstOrDefault().size;
                    Dups.Add(d);
                }
                /*
                                Console.WriteLine(
                                    "Duplicate Found - " + "[ " + potentialduplicate.Count + " ]" + item.path
                                );

                */
            }
        }
        // var g = Dups;
        //Console.ReadKey();
        Console.WriteLine("_____________________________________________________");
        foreach (var duplicate in Dups)
        {
            Console.WriteLine(
                duplicate.filename
                    + "  ["
                    + duplicate.count
                    + "] copies. "
                    + " Total size occupied - ["
                    + (duplicate.size / Constants.one_mb) * duplicate.paths.Count
                    + " MB] "
                    + " \nCan be reduced to ["
                    + (duplicate.size / Constants.one_mb)
                    + " MB] if the extra "
                    + (duplicate.paths.Count - 1)
                    + " copies are deleted"
            );

            Console.WriteLine("_____________________________________________________");
            foreach (var path in duplicate.paths)
            {
                Console.WriteLine("---- " + path);
            }
            Console.WriteLine("\n\n");
        }
        Console.WriteLine("_____________________________________________________");
        Console.ReadKey();
    }

    // Process all files in the directory passed in, recurse on any directories
    // that are found, and process the files they contain.
    public static void ProcessDirectory(string targetDirectory)
    {
        // Process the list of files found in the directory.
        string[] fileEntries = Directory.GetFiles(targetDirectory);
        foreach (string fileName in fileEntries)
            ProcessFile(fileName);

        // Recurse into subdirectories of this directory.
        string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
        foreach (string subdirectory in subdirectoryEntries)
            ProcessDirectory(subdirectory);
    }

    // Insert logic for processing found files here.
    public static void ProcessFile(string path)
    {
        long length = new System.IO.FileInfo(path).Length;
        Console.WriteLine("Processed file '{0}' - size - '{1}'.", path, length / Constants.one_mb);

        FileInformation temp = new FileInformation();
        temp.creationtime = File.GetCreationTime(path);
        temp.path = path;
        temp.size = length;
        temp.filename = Path.GetFileName(path);

        //Console.WriteLine(length);
        if (temp.size >= Constants.filter_size)
        {
            FI.Add(temp);
        }
        else
        {
            Console.WriteLine(
                "Skipped as file size is below req. - " + temp.size / Constants.one_mb + " MB"
            );
        }
        //return temp;
    }
}
