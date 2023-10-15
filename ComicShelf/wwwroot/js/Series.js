$.validator.addMethod("reqif",
    function (value, element, parameters) {
        var onging = $("#Series_Onging").is(":checked");
        return onging == false;
    });

$.validator.unobtrusive.adapters.add("reqif", [], function (options) {
    options.rules.reqif = {};
    options.messages["reqif"] = options.message;
});

$(function () {

    ongoingChanged();

    function split(val) {
        return val.split(/,\s*/);
    }
    function extractLast(term) {
        return split(term).pop();
    }

    $('#Series_Publishers').autocomplete({
        minLength: 0,
        source: function (request, response) {
            $.getJSON("?handler=Search", {
                term: extractLast(request.term)
            }, response);
        },
        focus: function () {
            // prevent value inserted on focus
            return false;
        },
        select: function (event, ui) {
            var terms = split(this.value);
            // remove the current input
            terms.pop();
            // add the selected item
            terms.push(ui.item.label);
            // add placeholder to get the comma-and-space at the end
            terms.push("");
            this.value = terms.join(", ");
            return false;
        }
    })

        .on("keydown", function (event) {
            if (event.keyCode === $.ui.keyCode.TAB &&
                $(this).autocomplete("instance").menu.active) {
                event.preventDefault();
            }
        })


})


function changeSeriesComplete(e) {

    if (e.status == 200) {
        console.log(e)
    }
    else {
        console.log(e)
    }
}

function OngoingChanged(cb) {
    console.log(cb);
}