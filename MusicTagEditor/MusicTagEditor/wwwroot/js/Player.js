var playButton = document.querySelector('#play');
var volumeSlider = document.querySelector('#volumeSlider');
var songIsPlaying = false;
var player = null;
var volume = 10;
var currentTime = 0;

var playerParams = {
    currentTime: 0,
    volume: 10,
    songIsPlaying: false,
    durration: 0,
    songName: ""
}

playButton.addEventListener('click', play);
volumeSlider.addEventListener('change', setVolume);

async function play(event) {
    try {
        const currentElem = document.querySelector('.active');
       
        if (isEmptyObject(currentElem)) return;

        const name = currentElem.textContent;

        if(playerParams.songIsPlaying && 
            player != null &&
            playerParams.songName == name)
        {
            player.pause();
            event.target.src = "/Icons/play.svg";
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
            event.target.src = "/Icons/pause.svg";
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
            player = AV.Player.fromBuffer(song);
            
            //player.watch("seekTime", changeSongTime);
            player.play();
            event.target.src = "/Icons/pause.svg";
            player.volume = playerParams.volume;
            playerParams.songIsPlaying = true;
            playerParams.durration = player.durration;
        } else {
            alert("HTTP Error: " + response.status);
        }
    }

    catch (error) {
        console.error("Error: ", error)
    }
}

function isEmptyString(str) {
    return (!str || 0 === str.length);
}

function updateUiTime(durration)
{
    let 
}

function seekSong() {
        
}

function changeSongTime(id, oldval, newval)
{
    console.log(newval);
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
        for (var i in obj) {
            if (obj.hasOwnProperty(i)) {
                return false;
            }
        }
        return true;
    }
}