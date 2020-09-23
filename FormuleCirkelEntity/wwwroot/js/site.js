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
    { id: 'AF', text: 'Afghanistan' },
    { id: 'AL', text: 'Albania' },
    { id: 'DZ', text: 'Algeria' },
    { id: 'AD', text: 'Andorra' },
    { id: 'AO', text: 'Angola' },
    { id: 'AI', text: 'Anguilla' },
    { id: 'AQ', text: 'Antarctica' },
    { id: 'AR', text: 'Argentina' },
    { id: 'AM', text: 'Armenia' },
    { id: 'AW', text: 'Aruba' },
    { id: 'AU', text: 'Australia' },
    { id: 'AT', text: 'Austria' },
    { id: 'AZ', text: 'Azerbaijan' },
    { id: 'BS', text: 'Bahamas' },
    { id: 'BH', text: 'Bahrain' },
    { id: 'BD', text: 'Bangladesh' },
    { id: 'BB', text: 'Barbados' },
    { id: 'BY', text: 'Belarus' },
    { id: 'BE', text: 'Belgium' },
    { id: 'BZ', text: 'Belize' },
    { id: 'BJ', text: 'Benin' },
    { id: 'BM', text: 'Bermuda' },
    { id: 'BT', text: 'Bhutan' },
    { id: 'BO', text: 'Bolivia' },
    { id: 'BA', text: 'Bosnia And Herzegovina' },
    { id: 'BW', text: 'Botswana' },
    { id: 'BR', text: 'Brazil' },
    { id: 'BN', text: 'Brunei Darussalam' },
    { id: 'BG', text: 'Bulgaria' },
    { id: 'BF', text: 'Burkina Faso' },
    { id: 'KH', text: 'Cambodia' },
    { id: 'CM', text: 'Cameroon' },
    { id: 'CA', text: 'Canada' },
    { id: 'CF', text: 'Central African Republic' },
    { id: 'TD', text: 'Chad' },
    { id: 'CL', text: 'Chile' },
    { id: 'CN', text: 'China' },
    { id: 'CO', text: 'Colombia' },
    { id: 'CG', text: 'Congo' },
    { id: 'CR', text: 'Costa Rica' },
    { id: 'HR', text: 'Croatia' },
    { id: 'CU', text: 'Cuba' },
    { id: 'CY', text: 'Cyprus' },
    { id: 'CZ', text: 'Czech Republic' },
    { id: 'DK', text: 'Denmark' },
    { id: 'DJ', text: 'Djibouti' },
    { id: 'DM', text: 'Dominica' },
    { id: 'DO', text: 'Dominican Republic' },
    { id: 'EC', text: 'Ecuador' },
    { id: 'EG', text: 'Egypt' },
    { id: 'SV', text: 'El Salvador' },
    { id: 'ER', text: 'Eritrea' },
    { id: 'EE', text: 'Estonia' },
    { id: 'ET', text: 'Ethiopia' },
    { id: 'FJ', text: 'Fiji' },
    { id: 'FI', text: 'Finland' },
    { id: 'FR', text: 'France' },
    { id: 'GM', text: 'Gambia' },
    { id: 'GE', text: 'Georgia' },
    { id: 'DE', text: 'Germany' },
    { id: 'GH', text: 'Ghana' },
    { id: 'GI', text: 'Gibraltar' },
    { id: 'GR', text: 'Greece' },
    { id: 'GL', text: 'Greenland' },
    { id: 'GD', text: 'Grenada' },
    { id: 'GU', text: 'Guam' },
    { id: 'GT', text: 'Guatemala' },
    { id: 'GN', text: 'Guinea' },
    { id: 'GY', text: 'Guyana' },
    { id: 'HT', text: 'Haiti' },
    { id: 'HN', text: 'Honduras' },
    { id: 'HK', text: 'Hong Kong' },
    { id: 'HU', text: 'Hungary' },
    { id: 'IS', text: 'Iceland' },
    { id: 'IN', text: 'India' },
    { id: 'ID', text: 'Indonesia' },
    { id: 'IR', text: 'Iran}, Islamic Republic Of' },
    { id: 'IQ', text: 'Iraq' },
    { id: 'IE', text: 'Ireland' },
    { id: 'IL', text: 'Israel' },
    { id: 'IT', text: 'Italy' },
    { id: 'JM', text: 'Jamaica' },
    { id: 'JP', text: 'Japan' },
    { id: 'JE', text: 'Jersey' },
    { id: 'JO', text: 'Jordan' },
    { id: 'KZ', text: 'Kazakhstan' },
    { id: 'KE', text: 'Kenya' },
    { id: 'KR', text: 'Korea' },
    { id: 'KW', text: 'Kuwait' },
    { id: 'LV', text: 'Latvia' },
    { id: 'LR', text: 'Liberia' },
    { id: 'LI', text: 'Liechtenstein' },
    { id: 'LT', text: 'Lithuania' },
    { id: 'LU', text: 'Luxembourg' },
    { id: 'MO', text: 'Macao' },
    { id: 'MK', text: 'Macedonia' },
    { id: 'MG', text: 'Madagascar' },
    { id: 'MY', text: 'Malaysia' },
    { id: 'MV', text: 'Maldives' },
    { id: 'ML', text: 'Mali' },
    { id: 'MT', text: 'Malta' },
    { id: 'MX', text: 'Mexico' },
    { id: 'FM', text: 'Micronesia}, Federated States Of' },
    { id: 'MD', text: 'Moldova' },
    { id: 'MC', text: 'Monaco' },
    { id: 'MN', text: 'Mongolia' },
    { id: 'ME', text: 'Montenegro' },
    { id: 'MA', text: 'Morocco' },
    { id: 'MZ', text: 'Mozambique' },
    { id: 'MM', text: 'Myanmar' },
    { id: 'NA', text: 'Namibia' },
    { id: 'NP', text: 'Nepal' },
    { id: 'NL', text: 'Netherlands' },
    { id: 'AN', text: 'Netherlands Antilles' },
    { id: 'NZ', text: 'New Zealand' },
    { id: 'NE', text: 'Niger' },
    { id: 'NG', text: 'Nigeria' },
    { id: 'NO', text: 'Norway' },
    { id: 'OM', text: 'Oman' },
    { id: 'PK', text: 'Pakistan' },
    { id: 'PA', text: 'Panama' },
    { id: 'PG', text: 'Papua New Guinea' },
    { id: 'PY', text: 'Paraguay' },
    { id: 'PE', text: 'Peru' },
    { id: 'PH', text: 'Philippines' },
    { id: 'PL', text: 'Poland' },
    { id: 'PT', text: 'Portugal' },
    { id: 'PR', text: 'Puerto Rico' },
    { id: 'QA', text: 'Qatar' },
    { id: 'RO', text: 'Romania' },
    { id: 'RU', text: 'Russian Federation' },
    { id: 'RW', text: 'Rwanda' },
    { id: 'SM', text: 'San Marino' },
    { id: 'SA', text: 'Saudi Arabia' },
    { id: 'SN', text: 'Senegal' },
    { id: 'RS', text: 'Serbia' },
    { id: 'SL', text: 'Sierra Leone' },
    { id: 'SG', text: 'Singapore' },
    { id: 'SK', text: 'Slovakia' },
    { id: 'SI', text: 'Slovenia' },
    { id: 'SO', text: 'Somalia' },
    { id: 'ZA', text: 'South Africa' },
    { id: 'ES', text: 'Spain' },
    { id: 'LK', text: 'Sri Lanka' },
    { id: 'SD', text: 'Sudan' },
    { id: 'SR', text: 'Suriname' },
    { id: 'SE', text: 'Sweden' },
    { id: 'CH', text: 'Switzerland' },
    { id: 'SY', text: 'Syrian Arab Republic' },
    { id: 'TW', text: 'Taiwan' },
    { id: 'TZ', text: 'Tanzania' },
    { id: 'TH', text: 'Thailand' },
    { id: 'TN', text: 'Tunisia' },
    { id: 'TR', text: 'Turkey' },
    { id: 'TM', text: 'Turkmenistan' },
    { id: 'UG', text: 'Uganda' },
    { id: 'UA', text: 'Ukraine' },
    { id: 'AE', text: 'United Arab Emirates' },
    { id: 'GB', text: 'United Kingdom' },
    { id: 'US', text: 'United States' },
    { id: 'UY', text: 'Uruguay' },
    { id: 'UZ', text: 'Uzbekistan' },
    { id: 'VE', text: 'Venezuela' },
    { id: 'VN', text: 'Vietnam' },
    { id: 'YE', text: 'Yemen' },
    { id: 'ZM', text: 'Zambia' },
    { id: 'ZW', text: 'Zimbabwe' }
];

function formatCountry(country) {
    if (!country.id) { return country.text; }
    var $country = $('<span class="flag flag-' + country.id.toLowerCase() + '></span>' +
        '<span class="flag-text">' + country.text + '</span>');
    return $country;
}
