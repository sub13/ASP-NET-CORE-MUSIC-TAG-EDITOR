var playButton = document.querySelector('#play');
var songIsPlaying = false;
var player = null;

playButton.addEventListener('click', play);

async function play(event) {
    try {

        if(songIsPlaying && player != null)
        {
            player.pause();
            songIsPlaying = false;
            event.target.src = "/Icons/play.svg";
            return;
        }

        /*
        if(player != null)
        {
            player.play();
            songIsPlaying = true;
            return;
        }*/

        const data = new FormData();
        const currentElem = document.querySelector('.active');

        if (isEmptyObject(currentElem)) return;

        const name = currentElem.textContent;

        data.append("name", name);

        var response = await fetch("GetSongByName", {
            method: "POST",
            body: data
        });

        if (response.ok) {
            let song = await response.arrayBuffer();
            player = AV.Player.fromBuffer(song);
            player.play();
            songIsPlaying = true;
            event.target.src = "/Icons/pause.svg";
        } else {
            alert("HTTP Error: " + response.status);
        }
    }

    catch (error) {
        console.error("Error: ", error)
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