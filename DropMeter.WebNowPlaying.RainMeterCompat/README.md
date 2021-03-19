# Web Now Playing for Rainmeter
A plugin for Rainmeter that when paired with a browser extension allows retrieval of music info and playback control from various websites such as soundcloud or youtube.  
Included in this repo is an example skin that shows how to use every measure and bang.  
Source code for the browser extension can be found [here](https://github.com/tjhrulz/WebNowPlaying-BrowserExtension)

## Extension links:
[Chrome Web Store](https://chrome.google.com/webstore/detail/webnowplaying-companion/jfakgfcdgpghbbefmdfjkbdlibjgnbli)  
[Firefox Addons Store](https://addons.mozilla.org/en-US/firefox/addon/webnowplaying-companion/)  
**[Browser extension source](https://github.com/tjhrulz/WebNowPlaying-BrowserExtension)**  
**Note:** While using Firefox you will need to go to the about:config page in Firefox and set network.websocket.allowInsecureFromHTTPS to true until I can look into using a self signed SSL Certifcate or have the funds to purchase one. 

### Current state:
Fully functional 

### Future additions: (See extension source code for web side addition)
- Improve autoswitcher
- Storing image in temp when no location is declared instead of in your Rainmeter directory
- Avoid redownloading image if it is the same as the last image (Or some sort of image caching)
- Using secure websockets for everything

#### If you would like you can donate to support the plugin [here](https://www.paypal.me/tjhrulz)

## Measure types:

- `Player`

  What the name of the player is the song is coming from. i.e Youtube or Soundcloud  
  
- `Title, Artist, Album`

  String of current playing songs info, blank if no info.    

- `Cover`

  String that points to current album art, while downloading or if no album art points to the path of the default.  
  **Note:** Do not assume the image will always be a square.
  
  **Attributes:**  
  CoverPath - A system path to where to store the album art.  
  DefaultPath - A system path to what image to use when downloading the album art.
  
- `CoverWebAddress`

  String of URL location of current album art, useful for doing an onChangeAction as cover will update twice when the song changes. This will only update once and only once the image has been downloaded to the disk.
  
- `Position, Duration`

  String of how far into the song or how long the song is, formated MM:SS.
  
- `Progress`

  Double of how far into the song you are as a percentage. To clarify that number is formated ##.##### and has a predefined max of 100.00.

- `Repeat`

  Integer of if the music player is set to repeat. 0 is no, 1 is repeat one song, 2 is repeat all.  
  **Note:** If unsupported by current website value will always be 0
  
- `Shuffle`

  Integer of if the music player is set to shuffle. 0 is no, 1 is yes.  
  **Note:** If unsupported by current website value will always be 0
  
- `Rating`

  Integer of the rating of the song. 0 through 5. Sites with binary rating system have Thumbs Down =1 Thumbs up =5. 0 is unrated.
  
- `Volume`
  
  Integer between 0-100 of what the current volume is set in the music player.
  
- `State`

  Integer of the play state of the music player. 0 is stopped, 1 is playing, 2 is paused.
  
- `Status`

  If 0 no supported websites are open, 1 is if one or more websites are open that are supported.  
  **Note:** Do not use this to see if the browser extension is installed. If there is enough demand I will try to add a way to check that in the future  

## Bangs:

- `SetPosition ##.####`

  Where ##.#### is a Double between 0-100. Sets the what percent of the way through the song the song is. Add + or - in front to set the position relatively.
  
- `SetVolume ###`

  Where ### is a Integer beween 0-100, add + or - in front to set the volume relatively.  
  **Note:** Some websites may not have the volume sliders change since I change the volume of the internal audio file.
  
- `Previous`, `PlayPause`, `Next`

  Self explanitory.
  **Note:** Previous on youtube currently is a little funky since I just move the page back one in the history if not using a playlist. I will add better tracking in the future.
  
- `Repeat`

  Toggles through repeat modes supported by websites. Sometimes may be none at all.
  
- `Shuffle`

  Toggles through shuffle modes supported by websites. Sometimes may be none at all.
  
- `ToggleThumbsUp` and `ToggleThumbsDown`

  Toggles the song being thumbed up or down, to set it to a specific state see SetRating.
  **Note:** Not all sites support thumbs down. Any site that uses stars will toggle the rating to be 5 stars and 1 star repspectively.
  
- `SetRating #`

  Where # is an integer, 0-5. Sites with binary rating system have Thumbs Down =1 Thumbs up =5. 0 is unrated.
