
 var fileContainerCopy = document.querySelector('[name = "FileContainer"]').cloneNode(true);
 var ChooseSongRow;
 
/*
var connectionId = "";
var hubConnection = new signalR.HubConnectionBuilder()
 .withUrl("/GetTag")
 .withAutomaticReconnect([0, 1000, 100000, null])
 .build();


hubConnection.on("GetTagForm", (data) => DisplayTagData(data));

hubConnection.start().then(() => {
    // после соединения получаем id подключения
    connectionId = hubConnection.connectionId;
});
*/

var DisplayTagData = (data) =>
{
    ChooseSongRow =  document.querySelector('[name = "ChooseSong"]');
    //fileContainer.parentNode.removeChild(fileContainer);
    let selectedSong = document.querySelector('.active');
    let nameSong = selectedSong.textContent;
    
    ChooseSongRow.innerHTML =`
    <form class="mt-3" asp-anti-forgery="true" name="EditMusicForm" method="post" action="/Menu/SaveTag" enctype="multipart/form-data">       
        <div>
            <div class="form-group">
                <input type="hidden" name="nameFileSong" value='${nameSong}'>
            </div>
            <div class="form-group">
                <img id="mainSongImage" class="img-fluid" />
                <input type="file" class="form-control-file" data-show-upload="true" name="mainSongImage" accept="image/jpeg,image/png" maxlength="10" >
            </div>
            <div class="form-group">
                <label  id="exampleInputEmail1" for="Album">Альбом</label>
                <input class="form-control" type="text" id="Album" name="Album" value="">
            </div>
            <div class="form-group">
                <label  id="exampleInputEmail1" for="Year">Год</label>
                <input class="form-control" type="text" id="Year" name="Year" value="">
            </div>
            <div class="form-group">
                <label  id="exampleInputEmail1" for="Name">Имя</label>
                <input class="form-control" type="text" id="Name" name="Name" value="">
            </div>
            <div class="form-group">
                <label  id="exampleInputEmail1" for="Disc">Диск</label>
                <input class="form-control" type="text" id="Disc" name="Disc" value="">
            </div>
            <div class="form-group">
                <label  id="exampleInputEmail1" for="Comment">Комментарии</label>
                <input class="form-control" type="text" id="Comment" name="Comment" value="">
            </div>
            <div class="form-group">
                <label  id="exampleInputEmail1" for="Track">Трек</label>
                <input class="form-control" type="text" id="Track" name="Track" value="">
            </div>
            <div class="form-group">
                <label  id="exampleInputEmail1" for="Lyrics">Текст</label>
                <textarea class="form-control" type="text" id="Lyrics" name="Lyrics" rows="3"></textarea>
            </div>
            <div class="form-group">
                <label  id="exampleInputEmail1" for="Artists">Исполнители</label>
                <input class="form-control" type="text" id="Artists"  name="Artists" value="">
            </div>
            <div class="form-group">
                <label  id="exampleInputEmail1" for="Genres">Жанры</label>
                <input class="form-control" type="text" id="Genres" name="Genres" value="">
            </div>
            <div class="form-group">
                <label  id="exampleInputEmail1" for="Compositors">Композиторы</label>
                <input class="form-control" type="text" id="Compositors"  name="Compositors" value="">
            </div>
            <div class="form-group">
                <input type="submit" value="Сохранить" class="btn btn-primary">
            </div>
        </div>  
    </form>`;
    
    let Album =  document.querySelector('[name = "Album"]');
    Album.value = data.album.name;
    
    let Name =  document.querySelector('[name = "Name"]');
    Name.value = data.name;

    let Disc =  document.querySelector('[name = "Disc"]');
    Disc.value = data.disc;

    let Comment =  document.querySelector('[name = "Comment"]');
    Comment.value = data.comment;

    let Track =  document.querySelector('[name = "Track"]');
    Track.value = data.track;

    let Lyrics =  document.querySelector('[name = "Lyrics"]');
    Lyrics.value = data.lyrics;

    let Year =  document.querySelector('[name = "Year"]');
    Year.value = data.album.year;

    // добавление пунктов выбора
    /*
    addOptionToSelect(data.artists,'#Artists');
    addOptionToSelect(data.genres,'#Genres');
    addOptionToSelect(data.compositors,'#Compositors');
    */

    // Добавление множественных данных
    addMultipleDataToInput(data.artists,'#Artists');
    addMultipleDataToInput(data.genres,'#Genres');
    addMultipleDataToInput(data.compositors,'#Compositors');
    /*
    artistsSelect =  document.querySelector('#Artists');

    for (let artist in data.artists)
    {
        let artistOption = document.createElement('option');
        artistOption.value = artist;
        artistsSelect.appendChild(artistOption);
    }
    */
    
    //Замена кнопки
    let UniversalBtn =  document.querySelector('[name = "UniversalBtn"]');
    UniversalBtn.firstElementChild.src = "/Icons/arrow-left.svg";
    UniversalBtn.onclick = BackToList;

    //Установка изображения
    setImageFromBase64(data.pictureData,'#mainSongImage','500','500');    
};


function setImageFromBase64(data, selector, height, width)
{
    let mainSongImage = document.querySelector(selector);
    mainSongImage.src = "data:image/png;base64," + data;
    mainSongImage.height = height;
    mainSongImage.width = width;
}

function addMultipleDataToInput(data, selector)
{
    const selectElem =  document.querySelector(selector);
    let str ='';
    let i = 0;
    for (let elem in data)
    {
        if (i == 0)
            str = data[elem].name;
        else
            str += `,${data[elem].name}`;
        i++;
    }
    selectElem.value = str;
}

async function SendPathToServer()
{
    try
    {
        const data = new FormData();
        const currentElem = document.querySelector('.active');
        
        if(isEmptyObject(currentElem)) return;
        /*
        const selector = `p[name='path'][id='${idCurrent}']`;
        const pWithPath =  document.querySelector(selector);
        const path = pWithPath.textContent;
        */
        const name = currentElem.textContent;

        data.append("name", name);
        var response = await fetch("GetTagFromMusic", {
            method: "POST",
            body: data
        })

        if(response.ok)
        {
            let dataJson = await response.json();
            DisplayTagData(dataJson);
        }
        else
        {
            console.error("HTTP Error: " + response.status);
        }
    }
    catch (error)
    {
        console.error("Error: ", error);
    }
}

function isEmptyObject(obj) {
    if(obj == null)
    {    
        return true
    }
    else
    {
        for (var i in obj) {
            if (obj.hasOwnProperty(i)) {
                return false;
        }
    }
        return true;
    }
}

function BackToList()
{
    let EditForm = document.querySelector('[name = "EditMusicForm"]');
    EditForm.parentNode.removeChild(EditForm);
    ChooseSongRow.appendChild(fileContainerCopy);
    let UniversalBtn =  document.querySelector('[name = "UniversalBtn"]');
    UniversalBtn.firstElementChild.src = "/Icons/pencil.svg";
    UniversalBtn.onclick = SendPathToServer;
}