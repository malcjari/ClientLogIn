$(document).ready(function () {

    //import { ajax } from "jquery";

    editpassword();
    editprofile();
    onChange();

})

//function onChange() {

//    $("#new-password-input").keydown(function () {

        

//        var str = $("#new-password-input").val();
//        console.log(str);

//        var xhttp;
//        if (str.length == 0) {
//            $("#change-text").innerHTML = "";
//            return;
//        }
//        xhttp = new XMLHttpRequest();
//        xhttp.onreadystatechange = function () {
//                    $("#change-text").innerHTML = "FÖR KORT!!";

//        };

//    })
//}

function onChange() {

    $("#new-password-input").keydown(function () {



        var str = $("#new-password-input").val();
        console.log(str);

        $("#change.text").load("/User/AdminProfile");

    })
}    


function editpassword() {

    $("#toggle-password-btn").click(function (e) {
        e.preventDefault();
        $("#password-card").toggle();
    })
}

function editprofile() 
{
    let Disabled = true;
    $("#edit-toggle-popup").click(function (e) {

        e.preventDefault();
        Disabled = !Disabled;
        console.log(Disabled);
        $("#username-input").attr("disabled", Disabled)
        $("#name-input").attr("disabled", Disabled)
        $("#streeNo-input").attr("disabled", Disabled)
        $("#zipCode-input").attr("disabled", Disabled)
        $("#city-input").attr("disabled", Disabled)
        $("#phoneNumber-input").attr("disabled", Disabled)
        $("#email-input").attr("disabled", Disabled)
        $("#Save-edit").toggle();
        if (Disabled == true) {
            $(this).removeClass("btn-danger");
            $(this).addClass("btn-info");
            $(this).html("Redigera")
        } else {
            $(this).removeClass("btn-info");
            $(this).addClass("btn-danger");
            $(this).html("Avbryt")
        }
    })


}