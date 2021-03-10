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

// Array of all countries with id and their name
var isoCountries = [
	{
		"id": "AF",
		"text": "Afghanistan"
	},
	{
		"id": "AL",
		"text": "Albania"
	},
	{
		"id": "DZ",
		"text": "Algeria"
	},
	{
		"id": "AS",
		"text": "American Samoa"
	},
	{
		"id": "AD",
		"text": "Andorra"
	},
	{
		"id": "AO",
		"text": "Angola"
	},
	{
		"id": "AG",
		"text": "Antigua and Barbuda"
	},
	{
		"id": "AR",
		"text": "Argentina"
	},
	{
		"id": "AM",
		"text": "Armenia"
	},
	{
		"id": "AW",
		"text": "Aruba"
	},
	{
		"id": "SH",
		"text": "Ascension Island"
	},
	{
		"id": "AU",
		"text": "Australia"
	},
	{
		"id": "AT",
		"text": "Austria"
	},
	{
		"id": "AZ",
		"text": "Azerbaijan"
	},
	{
		"id": "BS",
		"text": "Bahamas"
	},
	{
		"id": "BH",
		"text": "Bahrain"
	},
	{
		"id": "BD",
		"text": "Bangladesh"
	},
	{
		"id": "BB",
		"text": "Barbados"
	},
	{
		"id": "BY",
		"text": "Belarus"
	},
	{
		"id": "BE",
		"text": "Belgium"
	},
	{
		"id": "BZ",
		"text": "Belize"
	},
	{
		"id": "BJ",
		"text": "Benin"
	},
	{
		"id": "BM",
		"text": "Bermuda"
	},
	{
		"id": "BT",
		"text": "Bhutan"
	},
	{
		"id": "BO",
		"text": "Bolivia"
	},
	{
		"id": "BQ",
		"text": "Bonaire"
	},
	{
		"id": "BA",
		"text": "Bosnia And Herzegovina"
	},
	{
		"id": "BW",
		"text": "Botswana"
	},
	{
		"id": "BR",
		"text": "Brazil"
	},
	{
		"id": "BN",
		"text": "Brunei Darussalam"
	},
	{
		"id": "BG",
		"text": "Bulgaria"
	},
	{
		"id": "BF",
		"text": "Burkina Faso"
	},
	{
		"id": "BI",
		"text": "Burundi"
	},
	{
		"id": "CV",
		"text": "Cabo Verde"
	},
	{
		"id": "KH",
		"text": "Cambodia"
	},
	{
		"id": "CM",
		"text": "Cameroon"
	},
	{
		"id": "CA",
		"text": "Canada"
	},
	{
		"id": "CF",
		"text": "Central African Republic"
	},
	{
		"id": "TD",
		"text": "Chad"
	},
	{
		"id": "CL",
		"text": "Chile"
	},
	{
		"id": "CN",
		"text": "China"
	},
	{
		"id": "CX",
		"text": "Christmas Island"
	},
	{
		"id": "CO",
		"text": "Colombia"
	},
	{
		"id": "CG",
		"text": "Congo"
	},
	{
		"id": "CR",
		"text": "Costa Rica"
	},
	{
		"id": "CI",
		"text": "Cote D'Ivoire"
	},
	{
		"id": "HR",
		"text": "Croatia"
	},
	{
		"id": "CU",
		"text": "Cuba"
	},
	{
		"id": "CW",
		"text": "Curaçao"
	},
	{
		"id": "CY",
		"text": "Cyprus"
	},
	{
		"id": "CZ",
		"text": "Czech Republic"
	},
	{
		"id": "CD",
		"text": "Democratic Republic of Congo"
	},
	{
		"id": "DK",
		"text": "Denmark"
	},
	{
		"id": "DJ",
		"text": "Djibouti"
	},
	{
		"id": "DM",
		"text": "Dominica"
	},
	{
		"id": "DO",
		"text": "Dominican Republic"
	},
	{
		"id": "EC",
		"text": "Ecuador"
	},
	{
		"id": "EG",
		"text": "Egypt"
	},
	{
		"id": "SV",
		"text": "El Salvador"
	},
	{
		"id": "GQ",
		"text": "Equatorial Guinea"
	},
	{
		"id": "ER",
		"text": "Eritrea"
	},
	{
		"id": "EE",
		"text": "Estonia"
	},
	{
		"id": "ET",
		"text": "Ethiopia"
	},
	{
		"id": "EU",
		"text": "Europe"
	},
	{
		"id": "FO",
		"text": "Faroe Islands"
	},
	{
		"id": "FJ",
		"text": "Fiji"
	},
	{
		"id": "FI",
		"text": "Finland"
	},
	{
		"id": "FR",
		"text": "France"
	},
	{
		"id": "GA",
		"text": "Gabon"
	},
	{
		"id": "GM",
		"text": "Gambia"
	},
	{
		"id": "GE",
		"text": "Georgia"
	},
	{
		"id": "DE",
		"text": "Germany"
	},
	{
		"id": "GH",
		"text": "Ghana"
	},
	{
		"id": "GI",
		"text": "Gibraltar"
	},
	{
		"id": "GR",
		"text": "Greece"
	},
	{
		"id": "GL",
		"text": "Greenland"
	},
	{
		"id": "GD",
		"text": "Grenada"
	},
	{
		"id": "GP",
		"text": "Guadeloupe"
	},
	{
		"id": "GU",
		"text": "Guam"
	},
	{
		"id": "GT",
		"text": "Guatemala"
	},
	{
		"id": "GN",
		"text": "Guinea"
	},
	{
		"id": "GW",
		"text": "Guinea-Bissau"
	},
	{
		"id": "GY",
		"text": "Guyana"
	},
	{
		"id": "HT",
		"text": "Haiti"
	},
	{
		"id": "VA",
		"text": "Holy See (Vatican)"
	},
	{
		"id": "HN",
		"text": "Honduras"
	},
	{
		"id": "HK",
		"text": "Hong Kong"
	},
	{
		"id": "HU",
		"text": "Hungary"
	},
	{
		"id": "IS",
		"text": "Iceland"
	},
	{
		"id": "IN",
		"text": "India"
	},
	{
		"id": "ID",
		"text": "Indonesia"
	},
	{
		"id": "IR",
		"text": "Iran"
	},
	{
		"id": "IQ",
		"text": "Iraq"
	},
	{
		"id": "IE",
		"text": "Ireland"
	},
	{
		"id": "IM",
		"text": "Isle Of Man"
	},
	{
		"id": "IL",
		"text": "Israel"
	},
	{
		"id": "IT",
		"text": "Italy"
	},
	{
		"id": "JM",
		"text": "Jamaica"
	},
	{
		"id": "JP",
		"text": "Japan"
	},
	{
		"id": "JE",
		"text": "Jersey"
	},
	{
		"id": "JR",
		"text": "Jolly Roger"
	},
	{
		"id": "JO",
		"text": "Jordan"
	},
	{
		"id": "KZ",
		"text": "Kazakhstan"
	},
	{
		"id": "KE",
		"text": "Kenya"
	},
	{
		"id": "KI",
		"text": "Kiribati"
	},
	{
		"id": "XK",
		"text": "Kosovo"
	},
	{
		"id": "KW",
		"text": "Kuwait"
	},
	{
		"id": "KG",
		"text": "Kyrgyzstan"
	},
	{
		"id": "LA",
		"text": "Lao People's Democratic Republic"
	},
	{
		"id": "LV",
		"text": "Latvia"
	},
	{
		"id": "LB",
		"text": "Lebanon"
	},
	{
		"id": "LS",
		"text": "Lesotho"
	},
	{
		"id": "LR",
		"text": "Liberia"
	},
	{
		"id": "LY",
		"text": "Libya"
	},
	{
		"id": "LI",
		"text": "Liechtenstein"
	},
	{
		"id": "LT",
		"text": "Lithuania"
	},
	{
		"id": "LU",
		"text": "Luxembourg"
	},
	{
		"id": "MO",
		"text": "Macao"
	},
	{
		"id": "MG",
		"text": "Madagascar"
	},
	{
		"id": "MW",
		"text": "Malawi"
	},
	{
		"id": "MY",
		"text": "Malaysia"
	},
	{
		"id": "MV",
		"text": "Maldives"
	},
	{
		"id": "ML",
		"text": "Mali"
	},
	{
		"id": "MT",
		"text": "Malta"
	},
	{
		"id": "MR",
		"text": "Mauritania"
	},
	{
		"id": "MU",
		"text": "Mauritius"
	},
	{
		"id": "YT",
		"text": "Mayotte"
	},
	{
		"id": "MX",
		"text": "Mexico"
	},
	{
		"id": "FM",
		"text": "Micronesia"
	},
	{
		"id": "MD",
		"text": "Moldova"
	},
	{
		"id": "MC",
		"text": "Monaco"
	},
	{
		"id": "MN",
		"text": "Mongolia"
	},
	{
		"id": "ME",
		"text": "Montenegro"
	},
	{
		"id": "MS",
		"text": "Montserrat"
	},
	{
		"id": "MA",
		"text": "Morocco"
	},
	{
		"id": "MZ",
		"text": "Mozambique"
	},
	{
		"id": "MM",
		"text": "Myanmar"
	},
	{
		"id": "NA",
		"text": "Namibia"
	},
	{
		"id": "NP",
		"text": "Nepal"
	},
	{
		"id": "NL",
		"text": "Netherlands"
	},
	{
		"id": "NZ",
		"text": "New Zealand"
	},
	{
		"id": "NI",
		"text": "Nicaragua"
	},
	{
		"id": "NE",
		"text": "Niger"
	},
	{
		"id": "NG",
		"text": "Nigeria"
	},
	{
		"id": "NF",
		"text": "Norfolk Island"
	},
	{
		"id": "KP",
		"text": "North Korea"
	},
	{
		"id": "MK",
		"text": "North Macedonia"
	},
	{
		"id": "MP",
		"text": "Northern Mariana Islands"
	},
	{
		"id": "NO",
		"text": "Norway"
	},
	{
		"id": "OM",
		"text": "Oman"
	},
	{
		"id": "PK",
		"text": "Pakistan"
	},
	{
		"id": "PW",
		"text": "Palau"
	},
	{
		"id": "PS",
		"text": "Palestine"
	},
	{
		"id": "PA",
		"text": "Panama"
	},
	{
		"id": "PG",
		"text": "Papua New Guinea"
	},
	{
		"id": "PY",
		"text": "Paraguay"
	},
	{
		"id": "PE",
		"text": "Peru"
	},
	{
		"id": "PH",
		"text": "Philippines"
	},
	{
		"id": "PL",
		"text": "Poland"
	},
	{
		"id": "PT",
		"text": "Portugal"
	},
	{
		"id": "PR",
		"text": "Puerto Rico"
	},
	{
		"id": "QA",
		"text": "Qatar"
	},
	{
		"id": "RO",
		"text": "Romania"
	},
	{
		"id": "RU",
		"text": "Russian Federation"
	},
	{
		"id": "RW",
		"text": "Rwanda"
	},
	{
		"id": "RE",
		"text": "Réunion"
	},
	{
		"id": "LC",
		"text": "Saint Lucia"
	},
	{
		"id": "PM",
		"text": "Saint Pierre and Miquelon"
	},
	{
		"id": "WS",
		"text": "Samoa"
	},
	{
		"id": "SM",
		"text": "San Marino"
	},
	{
		"id": "ST",
		"text": "Sao Tome and Principe"
	},
	{
		"id": "SA",
		"text": "Saudi Arabia"
	},
	{
		"id": "SN",
		"text": "Senegal"
	},
	{
		"id": "RS",
		"text": "Serbia"
	},
	{
		"id": "SC",
		"text": "Seychelles"
	},
	{
		"id": "SL",
		"text": "Sierra Leone"
	},
	{
		"id": "SG",
		"text": "Singapore"
	},
	{
		"id": "SX",
		"text": "Sint Maarten"
	},
	{
		"id": "SK",
		"text": "Slovakia"
	},
	{
		"id": "SI",
		"text": "Slovenia"
	},
	{
		"id": "SB",
		"text": "Solomon Islands"
	},
	{
		"id": "SO",
		"text": "Somalia"
	},
	{
		"id": "ZA",
		"text": "South Africa"
	},
	{
		"id": "KR",
		"text": "South Korea"
	},
	{
		"id": "SS",
		"text": "South Sudan"
	},
	{
		"id": "ES",
		"text": "Spain"
	},
	{
		"id": "LK",
		"text": "Sri Lanka"
	},
	{
		"id": "SD",
		"text": "Sudan"
	},
	{
		"id": "SR",
		"text": "Suriname"
	},
	{
		"id": "SZ",
		"text": "Swaziland"
	},
	{
		"id": "SE",
		"text": "Sweden"
	},
	{
		"id": "CH",
		"text": "Switzerland"
	},
	{
		"id": "SY",
		"text": "Syrian Arab Republic"
	},
	{
		"id": "TW",
		"text": "Taiwan"
	},
	{
		"id": "TJ",
		"text": "Tajikistan"
	},
	{
		"id": "TZ",
		"text": "Tanzania"
	},
	{
		"id": "TH",
		"text": "Thailand"
	},
	{
		"id": "TL",
		"text": "Timor-Leste"
	},
	{
		"id": "TG",
		"text": "Togo"
	},
	{
		"id": "TT",
		"text": "Trinidad And Tobago"
	},
	{
		"id": "TN",
		"text": "Tunisia"
	},
	{
		"id": "TR",
		"text": "Turkey"
	},
	{
		"id": "TM",
		"text": "Turkmenistan"
	},
	{
		"id": "UG",
		"text": "Uganda"
	},
	{
		"id": "UA",
		"text": "Ukraine"
	},
	{
		"id": "AE",
		"text": "United Arab Emirates"
	},
	{
		"id": "GB",
		"text": "United Kingdom"
	},
	{
		"id": "UN",
		"text": "United Nations"
	},
	{
		"id": "US",
		"text": "United States"
	},
	{
		"id": "UY",
		"text": "Uruguay"
	},
	{
		"id": "UZ",
		"text": "Uzbekistan"
	},
	{
		"id": "VE",
		"text": "Venezuela"
	},
	{
		"id": "VN",
		"text": "Vietnam"
	},
	{
		"id": "VG",
		"text": "Virgin Islands"
	},
	{
		"id": "VI",
		"text": "Virgin Islands USA"
	},
	{
		"id": "EH",
		"text": "Western Sahara"
	},
	{
		"id": "YE",
		"text": "Yemen"
	},
	{
		"id": "ZM",
		"text": "Zambia"
	},
	{
		"id": "ZW",
		"text": "Zimbabwe"
	}
];

var bgs = [
	'/../images/backgrounds/camel_lotus_sato.jpg',
	'/../images/backgrounds/classic_ferrari_schumacher.jpg',
	'/../images/backgrounds/honda_earth_button.jpg',
	'/../images/backgrounds/kimi_sauberbull.jpeg',
	'/../images/backgrounds/manor_wehrlein.jpg',
	'/../images/backgrounds/marlboro_mclaren_mika.jpg',
	'/../images/backgrounds/mclaren_mika.jpg',
	'/../images/backgrounds/mercedes_bottas_kaputt.jpg',
	'/../images/backgrounds/mercedes_hamilton_kaputt.jpg',
	'/../images/backgrounds/mika_mclaren.jpg',
	'/../images/backgrounds/player_lotus_senna.jpg',
	'/../images/backgrounds/redbull_verstappen.jpg',
	'/../images/backgrounds/redbull_verstappen_ricciardo.jpg',
	'/../images/backgrounds/renault_alonso.jpg'
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