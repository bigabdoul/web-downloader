# Visual C# Web Resources Downloader

### Description
Web Resources Downloader (WRD) is a Visual C# solution that downloads images, files, and other resources from the web using asynchronous methods.

### Purpose
The goal of this project is to create a graphical user interface that provides light-weight functionalities similar to those of Download Accelerator Plus (DAP) and to help others learn asynchronous programming with Visual C# and the .NET Framework.

### Current features
As of this writing only a Windows Form for downloading images using the HtmlAgilityPack in the background has been created and allows various operations such as Pause/Resume, and Cancel current downloads.

The interface also allows importing a download list file, which is a simple text file with URLs to download. Optionally, a download list file can provide download destination folder hints for each group of URLs found in the file.

### Features to add
As mentioned earlier, basic DAP-like behavior is expected when it comes to downloading any web resource. Therefore, another project is expected to be added with base classes that will provide functionalities for accelerating downloads by establishing multiple connections to the same resource at different entry points and then merging those entry points into a single final downloaded file.

In the end, these core (non-exhaustive) features are absolutely must-haves:
1. Pause/Resume current file/group download
2. Skip current file/group download
3. Cancel current file/group download
4. Knowing the URL, resume any broken download

### Contributions
If you are interested in contributing to this project, either by submitting code, suggesting new features, or pointing out areas of improvement, please feel free to join.
