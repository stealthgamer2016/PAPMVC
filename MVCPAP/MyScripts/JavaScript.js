$(function () {
    $("#SearchQuery").change(function (textbox) {
        $("#formsearch").attr('action', '../../Home/SearchPage/' + $("#SearchQuery").val());
    });
    $(".username").click(function (span) {
        var User = span.toElement.textContent.replace('#', '-');
        window.location.assign("../../../Home/ProfilePage/" + User)
    });
    $(".Video").click(function (div) {
        var id = 0;
        if (div.toElement.classList == 'Video') {
            var id = div.toElement.id;
        }
        else {
            id = $("#" + div.toElement.id).closest(".Video").attr('id');
        }

        if (div.toElement.classList != 'username')
            window.location.assign("../../../Home/VideoPage/" + id);
    });


});