using System.Linq;

public class FileSystemManager : SingletonMonobehaviour<FileSystemManager>
{
    public VirtualDirectory RootDirectory { get; private set; }

    void OnEnable()
    {
        // This will always run, and be overwritten later if a save file exists.
        RootDirectory = new VirtualDirectory("/");
        var testDirectory = new VirtualDirectory("test");
        RootDirectory.AddFile(testDirectory);
        var testDirectory2 = new VirtualDirectory("test2");
        RootDirectory.AddFile(testDirectory2);
        testDirectory.AddFile(new VirtualImage("testfile.png", null));
        testDirectory2.AddFile(new VirtualImage("testfile2.png", null));
    }

    public VirtualDirectory FindDirectoryInRoot(string directoryName)
    {
        return RootDirectory.DirectoryFiles.FirstOrDefault(x => x.FileName == directoryName) as VirtualDirectory;
    }

    public bool TrySaveFile(string sampleID, IVirtualFile newFile)
    {
        var rootFiles = RootDirectory.DirectoryFiles;
        if (rootFiles.FirstOrDefault(x => x.FileName == sampleID) is not VirtualDirectory sampleDirectory)
        {
            sampleDirectory = new VirtualDirectory(sampleID);
            RootDirectory.AddFile(sampleDirectory);
        }

        if (sampleDirectory.DirectoryFiles.FirstOrDefault(x => x.FileName == newFile.FileName) != null)
            return false;
        
        sampleDirectory.AddFile(newFile);
        return true;
    }
}
