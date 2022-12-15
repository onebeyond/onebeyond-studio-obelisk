# How to run the SPA locally (first time)
1. Open your solution with Visual Studio 
2. Start the WebApi application as you would normally do (NOTE: ensure the WebApi is always up and running to make the SPA connect to your API successfully)
3. Note down somewhere your WebApi URL (it will be something like `https://localhost:PORT`)
4. Open Visual Studio Code and click on `File` -> `Open Folder...` and browse to the SPA main folder (the one containing `public`, `src`, etc)
5. Once your folder has been opened in VS Code, expand the `public` folder, then the `configuration` folder and open the `settings.dev.json` file for modifications
6. In the `settings.dev.json` replace the `apiUrl` value with the WebAPI URL you previously noted
7. As the website will run on HTTPS, we need to generate a self-signed certificate. So, open now a Terminal window and type the following commands:
    - `npm install -g mkcert`
    - `mkcert create-ca`
    - `mkcert create-cert`
8. Go to your SPA folder in Windows File Explorer, double click on the `ca.crt` file we just generated and a window will open. Perform the following steps:
    - Click on `Install Certificate...`
    - Keep the Store Location to `Current User`
    - Choose `Place all certificates in the following store`, browse and select `Trusted Root Certification Authorities`
    - Click on `Next` and then `Finish`
    - Click `Yes` on the next authorization window
9. Repeat the same steps above for the `cert.crt` file too
10. In the Terminal window run the following commands:
    - `npm install --save-dev webpack-cli`
    - `npm upgrade --save-dev webpack-cli`
11. In the Terminal window run `npm install`
12. Once the installation has completed, in the Terminal window you can finally run `npm run dev` and your SPA will open up in your preferred browser

# How to run the SPA locally (from the second time onwards)
1. Open the solution in Visual Studio and run the WebApi project
2. Open the SPA folder in Visual Studio Code, open a Terminal window and run `npm run dev`

# Can I run the SPA in Visual Studio?
Not recommended, just use Visual Studio Code

# Do I need to use the Windows Administration rights for any of the operations above?
No, your local user should be enough