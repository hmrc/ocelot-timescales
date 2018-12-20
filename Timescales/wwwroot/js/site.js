function getOwnerNames(page) {

    var ownerPid = $("#Owners").val();  
  
    if (ownerPid === "") {
        return;
    }

    ownerPid.split(",").forEach(function (pid) {

        $.when(getOwnerData(pid, page))
            .done(function (data) {
                $("#productOwnerName").append(data.name + " ");           
            })
            .fail(function () {
                $("#productOwnerName").append("PID not found ");    
            });
    });     
}

function getOwnerData(ownerPid, page) {

    var url;

    if (page === "Create") {
        url = "../../ad/api/loggedinuser/" + ownerPid;
    } else if (page === "Edit") {
        url = "../../../ad/api/loggedinuser/" + ownerPid;
    } else if (page === "Audit") {
        url = "../../ad/api/loggedinuser/" + ownerPid;
    }    
        
    return $.get(url);
}

function showUserData(ownerDetails) {

    if (ownerDetails === undefined) {
        $("#modalPid").val("PID not found");
        $("#modalName").val("Name not found");
        $("#modalPhone").val("Phone not found");
        $("#modalEmail").val("Email not found");
    } else {
        $("#modalPid").val(ownerDetails.pid);
        $("#modalName").val(ownerDetails.name);
        $("#modalPhone").val(ownerDetails.phoneNumber);
        $("#modalEmail").val(ownerDetails.email);
    }
}

$('#OwnerModal').on('show.bs.modal', function (event) {

    $("#modalPid").val("");
    $("#modalName").val("");
    $("#modalPhone").val("");
    $("#modalEmail").val("");

    var button = $(event.relatedTarget);
    var owner = button.data('whatever');
    var page = button.data('page');

    getOwnerData(owner, page)
        .done(function (data) {
            showUserData(data);
        })
        .fail(function () {
            showUserData();
        });
});