function tableComparer(index, isAscending) {
    let getCellValue = (row, idx) => row.children[idx].innerText || row.children[idx].textContent;
    let isNumber = (val) => val !== '' && !isNaN(val);
    return function (a, b) {
        let valA = getCellValue(isAscending ? a : b, index), valB = getCellValue(isAscending ? b : a, index);
        return isNumber(valA) && isNumber(valB) ? valA - valB : valA.toString().localeCompare(valB)
    }
}

function takeScreenshot(graphic, image) {
    html2canvas(document.querySelector(graphic)).then(function (canvas) {
        $(image).empty();
        document.querySelector(image).appendChild(canvas);
    });
}

// Used for development of all values
function getDev(number) {
    var element = document.getElementById("old-value-" + number);
    var text = element.textContent;
    var old = Number(text);
    var min = Math.ceil(document.getElementById("custom-min-" + number).value);
    var max = Math.floor(document.getElementById("custom-max-" + number).value);
    var dev = Math.floor(Math.random() * (max - min + 1)) + min;
    var newdev = (old + dev);

    document.getElementById("custom-change-" + number).innerText = dev;
    document.getElementById("custom-new-" + number).innerText = newdev;
}