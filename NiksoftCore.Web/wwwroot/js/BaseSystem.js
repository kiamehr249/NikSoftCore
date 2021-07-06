

function callApi(request) {
    $.post(request.url, request.data, function (data, status, xhr) {
        if (status === 'success') {
            request.success(data);
        } else {
            console.log('error: ' + xhr);
        }
    });
}

function dropdownBinder(sets) {
    $(sets.ddl).html('');
    $(sets.ddl).append($('<option></option>').val(0).html('انتخاب کنید'));
    sets.data.forEach(function (item, index) {
        $(sets.ddl).append($('<option></option>').val(item.id).html(item.title));
    });
}

function setItemDropdown(sets) {
    $(sets.ddl).val(sets.value);
}

function showMessage(sets) {
    new Noty({
        text: sets.text,
        type: sets.type,
        theme: 'sunset',
        progressBar: true,
        timeout: 4000
    }).show();
}

function addSeperator(str) {
    if (str === undefined || str == null || str === '') {
        return '';
    }
    return this.completeReplace(str.toString(), ',', '').replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

function completeReplace(str, find, replace) {
    if (str === undefined || str == null || str === '') {
        return '';
    }
    return str.replace(new RegExp(find, 'g'), replace);
}

function faDate(fullDate, type) {
    var dObj = new persianDate(fullDate).toLocale('fa');
    if (type == 1) {
        return dObj.format('MMMM DD YYYY');
    } else if (type == 2) {
        return dObj.format('MMMM DD YYYY, h:mm:ss a');
    } else {
        return dObj.format();
    }
}

function enDate(fullDate, type) {
    var dObj = new persianDate(fullDate).toLocale('en').toCalendar('gregorian');
    if (type == 1) {
        return dObj.format('MMMM DD YYYY');
    } else if (type == 2) {
        return dObj.format('YYYY-MM-DD h:mm:ss a');
    } else {
        return dObj.format();
    }
}

function faToEnNumbers(str) {
    if (str === undefined || str == null || str === '') {
        return '';
    }
    return str.toString().replace(/[۰-۹]/g, d => '۰۱۲۳۴۵۶۷۸۹'.indexOf(d));
}

function enToFaNumbers(str) {
    if (str === undefined || str == null || str === '') {
        return '';
    }
    return str.toString().replace(/\d/g, d => '۰۱۲۳۴۵۶۷۸۹'[d]);
}

$(document).ready(function(){
	tinymce.init({
        selector: '.text-editor',
		directionality : 'ltr',
		height : "500"
    });
});

