# VB.Net_Bayer_image_filter
For when you absolutely have to turn an image back into its R G B Bayer grids.

It's a simple command line tool for Windows.

**Bayeriser**       
Instructions:            
bayer.exe  /? or /help            : This help
bayer.exe  /split ""imageName""   : Splits file into 3: R, G, B
bayer.exe  /makeraw ""imageName"" : Uses the 3 files above to make RAW file
bayer.exe  /unraw ""imageName""   : Uses the raw file to make final image
bayer.exe  /all ""imageName""     : Do all steps above

![image](https://user-images.githubusercontent.com/1586332/126339127-dd0cbeb9-9f2b-4f8f-b912-7e1f91ffcd6b.png)
