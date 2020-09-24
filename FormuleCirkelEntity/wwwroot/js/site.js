// Enables tooltips
$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
});

function tableComparer(index, isAscending) {
    let getCellValue = (row, idx) => row.children[idx].innerText || row.children[idx].textContent;
    let isNumber = (val) => val !== '' && !isNaN(val);
    return function (a, b) {
        let valA = getCellValue(isAscending ? a : b, index), valB = getCellValue(isAscending ? b : a, index);
        return isNumber(valA) && isNumber(valB) ? valA - valB : valA.toString().localeCompare(valB)
    }
}

function takeScreenshot(graphic, imgContainer) {
    html2canvas(document.querySelector(graphic)).then(function (canvas) {
        $(imgContainer).empty();
        var image = new Image();
        image.src = canvas.toDataURL("image/jpeg");
        document.querySelector(imgContainer).appendChild(image);
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

// Makes all elements completely black which contain the correspending class
function hideElements() {
    var elementsToHide = document.getElementsByClassName('onclick-hide');
    for (var i = 0; i < elementsToHide.length; i++) {
        elementsToHide[i].style.backgroundColor = "black";
        elementsToHide[i].style.color = "black";
    }
}

var isoCountries = [
    { id: 'af', text: 'Afghanistan' },
    { id: 'al', text: 'Albania' },
    { id: 'dz', text: 'Algeria' },
    { id: 'ad', text: 'Andorra' },
    { id: 'ao', text: 'Angola' },
    { id: 'ai', text: 'Anguilla' },
    { id: 'aq', text: 'Antarctica' },
    { id: 'ar', text: 'Argentina' },
    { id: 'Am', text: 'Armenia' },
    { id: 'aw', text: 'Aruba' },
    { id: 'au', text: 'Australia' },
    { id: 'at', text: 'Austria' },
    { id: 'az', text: 'Azerbaijan' },
    { id: 'bs', text: 'Bahamas' },
    { id: 'bh', text: 'Bahrain' },
    { id: 'bd', text: 'Bangladesh' },
    { id: 'bb', text: 'Barbados' },
    { id: 'by', text: 'Belarus' },
    { id: 'be', text: 'Belgium' },
    { id: 'bz', text: 'Belize' },
    { id: 'bj', text: 'Benin' },
    { id: 'bm', text: 'Bermuda' },
    { id: 'bt', text: 'Bhutan' },
    { id: 'bo', text: 'Bolivia' },
    { id: 'ba', text: 'Bosnia And Herzegovina' },
    { id: 'bw', text: 'Botswana' },
    { id: 'br', text: 'Brazil' },
    { id: 'bn', text: 'Brunei Darussalam' },
    { id: 'bg', text: 'Bulgaria' },
    { id: 'bf', text: 'Burkina Faso' },
    { id: 'kh', text: 'Cambodia' },
    { id: 'cm', text: 'Cameroon' },
    { id: 'ca', text: 'Canada' },
    { id: 'cf', text: 'Central African Republic' },
    { id: 'td', text: 'Chad' },
    { id: 'cl', text: 'Chile' },
    { id: 'cn', text: 'China' },
    { id: 'co', text: 'Colombia' },
    { id: 'cg', text: 'Congo' },
    { id: 'cr', text: 'Costa Rica' },
    { id: 'hr', text: 'Croatia' },
    { id: 'cu', text: 'Cuba' },
    { id: 'cy', text: 'Cyprus' },
    { id: 'cz', text: 'Czech Republic' },
    { id: 'dk', text: 'Denmark' },
    { id: 'dj', text: 'Djibouti' },
    { id: 'dm', text: 'Dominica' },
    { id: 'do', text: 'Dominican Republic' },
    { id: 'ec', text: 'Ecuador' },
    { id: 'eg', text: 'Egypt' },
    { id: 'sv', text: 'El Salvador' },
    { id: 'er', text: 'Eritrea' },
    { id: 'ee', text: 'Estonia' },
    { id: 'et', text: 'Ethiopia' },
    { id: 'fj', text: 'Fiji' },
    { id: 'fi', text: 'Finland' },
    { id: 'fr', text: 'France' },
    { id: 'gm', text: 'Gambia' },
    { id: 'ge', text: 'Georgia' },
    { id: 'de', text: 'Germany' },
    { id: 'gh', text: 'Ghana' },
    { id: 'gi', text: 'Gibraltar' },
    { id: 'gr', text: 'Greece' },
    { id: 'gl', text: 'Greenland' },
    { id: 'gd', text: 'Grenada' },
    { id: 'gu', text: 'Guam' },
    { id: 'gt', text: 'Guatemala' },
    { id: 'gn', text: 'Guinea' },
    { id: 'gy', text: 'Guyana' },
    { id: 'ht', text: 'Haiti' },
    { id: 'hn', text: 'Honduras' },
    { id: 'hk', text: 'Hong Kong' },
    { id: 'hu', text: 'Hungary' },
    { id: 'is', text: 'Iceland' },
    { id: 'in', text: 'India' },
    { id: 'id', text: 'Indonesia' },
    { id: 'ir', text: 'Iran}, Islamic Republic Of' },
    { id: 'iq', text: 'Iraq' },
    { id: 'ie', text: 'Ireland' },
    { id: 'il', text: 'Israel' },
    { id: 'it', text: 'Italy' },
    { id: 'jm', text: 'Jamaica' },
    { id: 'jp', text: 'Japan' },
    { id: 'je', text: 'Jersey' },
    { id: 'jo', text: 'Jordan' },
    { id: 'kz', text: 'Kazakhstan' },
    { id: 'ke', text: 'Kenya' },
    { id: 'kr', text: 'Korea' },
    { id: 'kw', text: 'Kuwait' },
    { id: 'lv', text: 'Latvia' },
    { id: 'lr', text: 'Liberia' },
    { id: 'li', text: 'Liechtenstein' },
    { id: 'lt', text: 'Lithuania' },
    { id: 'lu', text: 'Luxembourg' },
    { id: 'mo', text: 'Macao' },
    { id: 'mk', text: 'Macedonia' },
    { id: 'mg', text: 'Madagascar' },
    { id: 'my', text: 'Malaysia' },
    { id: 'mv', text: 'Maldives' },
    { id: 'ml', text: 'Mali' },
    { id: 'mt', text: 'Malta' },
    { id: 'mx', text: 'Mexico' },
    { id: 'fm', text: 'Micronesia}, Federated States Of' },
    { id: 'md', text: 'Moldova' },
    { id: 'mc', text: 'Monaco' },
    { id: 'mn', text: 'Mongolia' },
    { id: 'me', text: 'Montenegro' },
    { id: 'ma', text: 'Morocco' },
    { id: 'mz', text: 'Mozambique' },
    { id: 'mm', text: 'Myanmar' },
    { id: 'na', text: 'Namibia' },
    { id: 'np', text: 'Nepal' },
    { id: 'nl', text: 'Netherlands' },
    { id: 'an', text: 'Netherlands Antilles' },
    { id: 'nz', text: 'New Zealand' },
    { id: 'ne', text: 'Niger' },
    { id: 'ng', text: 'Nigeria' },
    { id: 'no', text: 'Norway' },
    { id: 'om', text: 'Oman' },
    { id: 'pk', text: 'Pakistan' },
    { id: 'pa', text: 'Panama' },
    { id: 'pg', text: 'Papua New Guinea' },
    { id: 'py', text: 'Paraguay' },
    { id: 'pe', text: 'Peru' },
    { id: 'ph', text: 'Philippines' },
    { id: 'pl', text: 'Poland' },
    { id: 'pt', text: 'Portugal' },
    { id: 'pr', text: 'Puerto Rico' },
    { id: 'qa', text: 'Qatar' },
    { id: 'ro', text: 'Romania' },
    { id: 'ru', text: 'Russian Federation' },
    { id: 'rw', text: 'Rwanda' },
    { id: 'sm', text: 'San Marino' },
    { id: 'sa', text: 'Saudi Arabia' },
    { id: 'sn', text: 'Senegal' },
    { id: 'rs', text: 'Serbia' },
    { id: 'sl', text: 'Sierra Leone' },
    { id: 'sg', text: 'Singapore' },
    { id: 'sk', text: 'Slovakia' },
    { id: 'si', text: 'Slovenia' },
    { id: 'so', text: 'Somalia' },
    { id: 'za', text: 'South Africa' },
    { id: 'es', text: 'Spain' },
    { id: 'lk', text: 'Sri Lanka' },
    { id: 'sd', text: 'Sudan' },
    { id: 'sr', text: 'Suriname' },
    { id: 'se', text: 'Sweden' },
    { id: 'ch', text: 'Switzerland' },
    { id: 'sy', text: 'Syrian Arab Republic' },
    { id: 'tw', text: 'Taiwan' },
    { id: 'tz', text: 'Tanzania' },
    { id: 'th', text: 'Thailand' },
    { id: 'tn', text: 'Tunisia' },
    { id: 'tr', text: 'Turkey' },
    { id: 'tm', text: 'Turkmenistan' },
    { id: 'ug', text: 'Uganda' },
    { id: 'ua', text: 'Ukraine' },
    { id: 'ae', text: 'United Arab Emirates' },
    { id: 'gb', text: 'United Kingdom' },
    { id: 'us', text: 'United States' },
    { id: 'uy', text: 'Uruguay' },
    { id: 'uz', text: 'Uzbekistan' },
    { id: 've', text: 'Venezuela' },
    { id: 'vn', text: 'Vietnam' },
    { id: 'ye', text: 'Yemen' },
    { id: 'zm', text: 'Zambia' },
    { id: 'zw', text: 'Zimbabwe' }
];
