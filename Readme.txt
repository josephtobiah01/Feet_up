

There are several readme files, each containing links and info about various topics, good practice to link an article that has info on a particual matter   


ANDROID:


debugger:
edge://inspect/#devices


font - case sensitive

For downloading set conent header to :

'apk' => application/vnd.android.package-archive


https://learn.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/shell/create


https://devblogs.microsoft.com/dotnet/performance-improvements-in-dotnet-maui/#record-a-custom-aot-profile


R8 option breaks the app



https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-7.0


Warning: unable to build chain to self-signed root for signer "Mac Developer: Dylan M (*******)"

Identifying the bad certificate:



From you Keychains select Login
From Category select Certificates
Find any Apple Certificate that has the blue +
Double click on the certificate.
Expand the Trust
If it's messed up then the "When using this certificate" is set to "Always Trust" along with the blue +
Fixing the bad certificate:

Just set it to "Use System Defaults" and close it.
You'll get a pop up. Type in your password to update settings.
Close KeyChain. Go back to your project, clean and run. Problem should have gone away.
If that didn't work then go back to Keychain and just double check and see if there are any other Apple certificates that are set to Always Trust and repeat the process.

If none of the above works, download the WWDR Intermediate Certificate and drag and drop into your 'System' keychain. Don't put it in 'login' - I tried that myself and it did nothing to help me. Try to avoid changing the trust settings of your certificates, Xcode gave me a warning when it saw I had modified my development certificate's trust settings.



FOR APPSTORE UPLOAD (PUBLISH->PACK) YOU NEED TO SELECT THE DISTRIBUTION PROFILE

USE THE "TRANSPORTER" MAC APP TO SEE THE VERIFICATION ERROR (i.e same version number....)



Manual provisioning:

You can download provision profiles from apple dev

copy them into :\Users\thoma\AppData\Local\Xamarin\iOS\Provisioning\Profiles
same to get rid of revoked certificates: \Users\thoma\AppData\Local\Xamarin\iOS\Provisioning\Certificates

MAUI Dependency Injecton (singetons)
-- viewmodel
-- model
https://www.youtube.com/watch?v=xx1mve2AQr4

Samplecode for the video
https://github.com/jamesmontemagno/MauiApp-DI?WT.mc_id=friends-0000-jamont


More text tutorials about this with code
https://www.syncfusion.com/blogs/post/learn-how-to-use-dependency-injection-in-net-maui.aspx


https://learn.microsoft.com/en-us/answers/questions/1167870/net-maui-how-to-run-method-after-page-is-shown