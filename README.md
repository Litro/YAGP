# YAGP
---------

## Configuration

Configuring the updater will be done in a file called Patcher.xml alongside the main executable, it must contain the following key/value combinations:

| Key | Value |
| ----|------ |
| **GameName** | Contains the name that will appear in the titlebar of your updater. |
| **RemoteDirectory** | Contains the exact location on your webserver where the updater will look for its files. (e.g. http://google.com/folder/ the final / is important!) |
| **NewsPage** | Contains the url that will be loaded in the webbrowser window. |
| **GameClient** | Contains the name of your executable, in case you do not wish to call it something generic. |

The resulting file would look similar to the following: http://pastebin.com/Vm2njkq6

**Note that these are all case sensitive!**

## Usage

### Preparing an Update

1. Open the Hash Calculator.
2. Select your Game Client folder (including the latest update, minus the patcher!)
3. Hit Go! and wait for it to finish.
4. Copy **output.xml** from the Hash Calculator folder to the folder you selected earlier and rename **output.xml** to **Checksums.xml**
5. Upload the entire folder you selected earlier to the location you pointed your updater to.

### Getting an Update

1. Make sure you have completed the previous steps, and configured your Patcher.
2. Run your patcher and sit back!
