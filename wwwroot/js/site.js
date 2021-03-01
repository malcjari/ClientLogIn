$(document).ready(function () {

   
    editpassword();
    editprofile(); 

})


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
        $("#name-input").attr("disabled", Disabled)
        $("#streeNo-input").attr("disabled", Disabled)
        $("#zipCode-input").attr("disabled", Disabled)
        $("#city-input").attr("disabled", Disabled)
        $("#phoneNumber-input").attr("disabled", Disabled)
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