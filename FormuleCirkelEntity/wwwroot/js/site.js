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

// Return random background
function returnRandomBackground() {
    var n = Math.floor(Math.random() * bgs.length);
    return bgs[n];
}

// Return random man
function returnRandomMan() {
    var n = Math.floor(Math.random() * man.length);
    return man[n];
}


var isoCountries = [
    { id: 'af', text: 'Afghanistan' },
    { id: 'al', text: 'Albania' },
    { id: 'dz', text: 'Algeria' },
    { id: 'as', text: 'American Samoa' },
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
    { id: 'io', text: 'British Indian Ocean Territory' },
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
    { id: 'cx', text: 'Christmas Island' },
    { id: 'co', text: 'Colombia' },
    { id: 'cg', text: 'Congo' },
    { id: 'cd', text: 'Democratic Republic of Congo' },
    { id: 'ck', text: 'Cook Islands' },
    { id: 'cr', text: 'Costa Rica' },
    { id: 'ci', text: 'Cote D\'Ivoire' },
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
    { id: 'fk', text: 'Falkland Islands' },
    { id: 'fo', text: 'Faroe Islands' },
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
    { id: 'va', text: 'Holy See (Vatican)' },
    { id: 'hn', text: 'Honduras' },
    { id: 'hk', text: 'Hong Kong' },
    { id: 'hu', text: 'Hungary' },
    { id: 'is', text: 'Iceland' },
    { id: 'in', text: 'India' },
    { id: 'id', text: 'Indonesia' },
    { id: 'ir', text: 'Iran' },
    { id: 'iq', text: 'Iraq' },
    { id: 'ie', text: 'Ireland' },
    { id: 'im', text: 'Isle Of Man' },
    { id: 'il', text: 'Israel' },
    { id: 'it', text: 'Italy' },
    { id: 'jm', text: 'Jamaica' },
    { id: 'jp', text: 'Japan' },
    { id: 'je', text: 'Jersey' },
    { id: 'jo', text: 'Jordan' },
    { id: 'kz', text: 'Kazakhstan' },
    { id: 'ke', text: 'Kenya' },
    { id: 'ki', text: 'Kiribati' },
    { id: 'kr', text: 'Korea' },
    { id: 'kw', text: 'Kuwait' },
    { id: 'kg', text: 'Kyrgyzstan' },
    { id: 'la', text: 'Lao People\'s Democratic Republic' },
    { id: 'lv', text: 'Latvia' },
    { id: 'lb', text: 'Lebanon' },
    { id: 'ls', text: 'Lesotho' },
    { id: 'lr', text: 'Liberia' },
    { id: 'li', text: 'Liechtenstein' },
    { id: 'lt', text: 'Lithuania' },
    { id: 'lu', text: 'Luxembourg' },
    { id: 'mo', text: 'Macao' },
    { id: 'mk', text: 'Macedonia' },
    { id: 'mg', text: 'Madagascar' },
    { id: 'mw', text: 'Malawi' },
    { id: 'my', text: 'Malaysia' },
    { id: 'mv', text: 'Maldives' },
    { id: 'ml', text: 'Mali' },
    { id: 'mt', text: 'Malta' },
    { id: 'mr', text: 'Mauritania' },
    { id: 'mx', text: 'Mexico' },
    { id: 'fm', text: 'Micronesia' },
    { id: 'md', text: 'Moldova' },
    { id: 'mc', text: 'Monaco' },
    { id: 'mn', text: 'Mongolia' },
    { id: 'me', text: 'Montenegro' },
    { id: 'ms', text: 'Montserrat' },
    { id: 'ma', text: 'Morocco' },
    { id: 'mz', text: 'Mozambique' },
    { id: 'mm', text: 'Myanmar' },
    { id: 'na', text: 'Namibia' },
    { id: 'np', text: 'Nepal' },
    { id: 'nl', text: 'Netherlands' },
    { id: 'an', text: 'Netherlands Antilles' },
    { id: 'nz', text: 'New Zealand' },
    { id: 'ni', text: 'Nicaragua' },
    { id: 'ne', text: 'Niger' },
    { id: 'ng', text: 'Nigeria' },
    { id: 'mp', text: 'Northern Mariana Islands' },
    { id: 'no', text: 'Norway' },
    { id: 'om', text: 'Oman' },
    { id: 'pk', text: 'Pakistan' },
    { id: 'pw', text: 'Palau' },
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
    { id: 'ws', text: 'Samoa' },
    { id: 'sm', text: 'San Marino' },
    { id: 'sa', text: 'Saudi Arabia' },
    { id: 'sn', text: 'Senegal' },
    { id: 'rs', text: 'Serbia' },
    { id: 'sc', text: 'Seychelles' },
    { id: 'sl', text: 'Sierra Leone' },
    { id: 'sg', text: 'Singapore' },
    { id: 'sk', text: 'Slovakia' },
    { id: 'si', text: 'Slovenia' },
    { id: 'sb', text: 'Solomon Islands' },
    { id: 'so', text: 'Somalia' },
    { id: 'za', text: 'South Africa' },
    { id: 'es', text: 'Spain' },
    { id: 'lk', text: 'Sri Lanka' },
    { id: 'sd', text: 'Sudan' },
    { id: 'sr', text: 'Suriname' },
    { id: 'sz', text: 'Swaziland' },
    { id: 'se', text: 'Sweden' },
    { id: 'ch', text: 'Switzerland' },
    { id: 'sy', text: 'Syrian Arab Republic' },
    { id: 'tw', text: 'Taiwan' },
    { id: 'tj', text: 'Tajikistan' },
    { id: 'tz', text: 'Tanzania' },
    { id: 'th', text: 'Thailand' },
    { id: 'tl', text: 'Timor-Leste' },
    { id: 'tg', text: 'Togo' },
    { id: 'tt', text: 'Trinidad And Tobago' },
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
    { id: 'vi', text: 'Virgin Islands USA' },
    { id: 'ye', text: 'Yemen' },
    { id: 'zm', text: 'Zambia' },
    { id: 'zw', text: 'Zimbabwe' }
];

var bgs = [
    '/../images/backgrounds/AlfaRomeoAss.jpg',
    '/../images/backgrounds/FerrariFutureTrip.jpeg',
    '/../images/backgrounds/HaasCurrentTrippy.jpg',
    '/../images/backgrounds/HakkinenUnderMoon.jpg',
    '/../images/backgrounds/HamiltonLeclercCrash.jpg',
    '/../images/backgrounds/HamiltonVerstappenMonaco.jpg',
    '/../images/backgrounds/KimiFerrariNightLight.jpg',
    '/../images/backgrounds/McLarenFinish.jpeg',
    '/../images/backgrounds/McLarenFutureAss.jpg',
    '/../images/backgrounds/McLarenUpFuture.jpg',
    '/../images/backgrounds/McLarenWest20xx.jpg',
    '/../images/backgrounds/MikaMcLarenJump.jpg',
    '/../images/backgrounds/RedBullRain.jpg',
    '/../images/backgrounds/ToroRossoSide.jpg',
    '/../images/backgrounds/VerstappenGermanyWin.jpg',
    '/../images/backgrounds/VettelNumberOne.jpg',
    '/../images/backgrounds/VettelPitstop.jpg',
    '/../images/backgrounds/VettelVerstappenCrash.jpg',
    '/../images/backgrounds/CharlesLeCrash.jpg'
];

var man = [
    '/../images/man/MAN.png',
    '/../images/man/man2.png',
    '/../images/man/man3.jpg',
    '/../images/man/man4.png',
    '/../images/man/men.png',
    '/../images/man/myhomie.jpg',
    '/../images/man/myman.jpg',
    '/../images/man/Pein.png'
];