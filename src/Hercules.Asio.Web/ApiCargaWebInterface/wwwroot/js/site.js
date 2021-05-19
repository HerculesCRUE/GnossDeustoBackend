// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {

    //peticion ajax a 
    ///autocomplete/GetUrlSearch
    //
    $('#formautocompletelds').attr('action', 'xx')

    $.ajax({
        url: "/carga-web/autocomplete/GetUrlSearch",
        type: "GET",
        dataType: "json",
        success: function (data) {
            $('#formautocompletelds').attr('action', data);
        }
    })

    $("#autocompletelds").autocomplete({
        minLength: 3,
        source: function (request, response) {
            $.ajax({
                url: "/carga-web/autocomplete",
                type: "GET",
                dataType: "json",
                data: {
                    q: request.term
                },
                success: function (data) {
                    response($.map(data, function (item) {
                        return {
                            label: item.Key,
                            url: item.Value
                        }
                    }))
                }
            })
        },
        select: function (event, ui) {
            document.location.href = ui.item.url;
        }
    })
});

function replaceAll(string, search, replace) {
    return string.split(search).join(replace);
}