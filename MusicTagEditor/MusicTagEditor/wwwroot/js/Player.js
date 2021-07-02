var playButton = document.querySelector('#play');
var volumeSlider = document.querySelector('#volumeSlider');
var timeSongSlider = document.querySelector('#timeSong');
var labelForTimeSong = document.querySelector("#labelTimeSong");
var songIsPlaying = false;
var player = null;
var volume = 10;
var currentTime = 0;

var playerParams = {
    currentTime: 0,
    volume: 10,
    songIsPlaying: false,
    songLength: 0,
    songName: ""
}

playButton.addEventListener('click', play);
volumeSlider.addEventListener('change', setVolume);
timeSongSlider.addEventListener('change', setTimeForSong);

async function play(event) {
    try {
        let currentElem = document.querySelector('.active');
        let name = ""
        
        if (isEmptyObject(currentElem)) 
        {
           currentElem =  document.querySelector('[name = "nameFileSong"]');
           if(isEmptyObject(currentElem))
                return;
            name = currentElem.value;    
        }
        else
        {
            name = currentElem.textContent;
        }

        let playButtonImage = document.querySelector("#playButtonImage");

        if(playerParams.songIsPlaying && 
            player != null &&
            playerParams.songName == name)
        {
            player.pause();
            playButtonImage.src = "/Icons/play.svg";
            playerParams.songIsPlaying = false;
            return;
        }

        const data = new FormData();

        if(player != null && 
            !isEmptyString(playerParams.songName) &&
            playerParams.songName == name &&
            !playerParams.songIsPlaying)
        {

            player.play();
            playButtonImage.src = "/Icons/pause.svg";
            playerParams.songIsPlaying = true;
            return;
        }

        playerParams.songName = name;

        data.append("name", name);

        var response = await fetch("GetSongByName", {
            method: "POST",
            body: data
        });

        if (response.ok) {
            let song = await response.arrayBuffer();
            if(player != null)
                player.destroy();
            player = await AV.Player.fromBuffer(song);
            //player.watch("seekTime", changeSongTime);
            player.play();
            playButtonImage.src = "/Icons/pause.svg";
            player.volume = playerParams.volume;
            playerParams.songIsPlaying = true;
            timeSongSlider.setAttribute("max", player.duration);
            player.watch("currentTime", updateUiTime);
            player.watch("duration", getCurrentSongLenght);
        } else {
            console.error("HTTP Error: " + response.status);
        }
    }

    catch (error) {
        console.error("Error: ", error)
    }
}

function isEmptyString(str) {
    return (!str || 0 === str.length);
}

function getCurrentSongLenght(id, oldVal, newVal)
{
    playerParams.songLength = newVal;
    timeSongSlider.setAttribute("max", playerParams.songLength);
}

function updateUiTime(id, oldVal, newVal)
{
    if(newVal != null && !isNaN(newVal))
    { 
        playerParams.currentTime = newVal;
        timeSongSlider.value = newVal;
        let commonTime = (playerParams.currentTime / 1000);
        let seconds = Math.floor(commonTime % 60);
        let minutes = Math.floor((commonTime /= 60) % 60);
        let time = "";
        if(seconds < 10)
            time = `${minutes}:${0 + seconds.toString()}`;
        else 
            time = `${minutes}:${seconds}`;
        //let valueTime = (newVal / 60000).toFixed(2);
        //let time = `${valueTime.trunc}:${String(valueTime).split('.')[1]}`
        labelForTimeSong.textContent = time;
    }
}

function setTimeForSong(event)
{
    if(player != null)
    {
        playerParams.currentTime = parseInt(event.target.value);
        player.unwatch("currentTime", updateUiTime);
        player.seek(playerParams.currentTime);
        player.watch("currentTime", updateUiTime);
    }
}

function setVolume(event){
    if(player != null)
    {
        playerParams.volume = event.target.value;
        player.volume = playerParams.volume;
    }
}

function isEmptyObject(obj) {
    if (obj == null) {
        return true
    }
    else {
        /*
        for (var i in obj) {
            if (obj.hasOwnProperty(i)) {
                return false;
            }
        }
        return true;
        */
       return false;
    }
}