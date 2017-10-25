# Web Crawler
Web Crawler - c# console application that recursively traverse and download a web site. 

The app focuses only on the HTML and Image files , the other scripts (css ..) are intentionally ignored. 
The app only downloads images and files on the same domain (the one specified) any other domains (blog ,cdn ...) are intentionally ignored. 

The application uses Rx.Net, future improvement would be to use Akka.Net.

The files are stored on the disk (folder with the binaries) ,  the folder is displayed in the console before and after the processing is done. 



  


