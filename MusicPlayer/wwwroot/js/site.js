function setDefaultImage(img, defaultImgName) {
    img.onerror = null;
    console.log(defaultImgName)
    img.src = "/DefaultPictures/" + defaultImgName;
}