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
        url = "../../../ad/api/loggedinuser/" + ownerPid;
    } else if (page === "Details") {
        url = "../../../ad/api/loggedinuser/" + ownerPid;
    }     
        
    return $.get(url);
}

function addUserData(ownerDetails) {

    if (ownerDetails === undefined) {
        $("#body").append(
            '<div class="card">' +
                '<div class= "card-body">' +
                    '<label class= "control-label" >PID</label>' +
                    '<input id="modalPid" class="form-control" disabled value="PID not found"/>' +

                    '<label class="control-label">Name</label>' +
                    '<input id="modalName" class="form-control" disabled value="PID not found"/>' +

                    '<label class="control-label">Phone</label>' +
                    '<input id="modalPhone" class="form-control" disabled value="PID not found"/>' +

                    '<label class="control-label">Email</label>' +
                    '<input id="modalEmail" class="form-control" disabled value="PID not found"/>' +
                '</div>' +
            '</div>' +

            '<br />'
        ); 
    } else {
        $("#body").append(
            '<div class="card">' +
                '<div class= "card-body">' +
                    '<label class= "control-label" >PID</label>' +
                    '<input id="modalPid" class="form-control" disabled value="' + ownerDetails.pid + '"/>' +

                    '<label class="control-label">Name</label>' +
                    '<input id="modalName" class="form-control" disabled value="' + ownerDetails.name + '"/>' +

                    '<label class="control-label">Phone</label>' +
                    '<input id="modalPhone" class="form-control" disabled value="' + ownerDetails.phoneNumber + '"/>' +

                    '<label class="control-label">Email</label>' +
                    '<input id="modalEmail" class="form-control" disabled value="' + ownerDetails.email + '"/>' +
                '</div>' +
            '</div>' +

            '<br />'
        ); 
    }
}

$('#OwnerModalList').on('show.bs.modal', function (event) {

    $("#body").empty();

    var button = $(event.relatedTarget);
    var owners = button.data('whatever');
    var page = button.data('page');

    if (owners.length > 7) {
        owners.split(",").forEach(function (pid) {
            $.when(getOwnerData(pid, page))
                .done(function (data) {
                    addUserData(data);
                })
                .fail(function () {
                    addUserData();
                });
        });
    } else {
        $.when(getOwnerData(owners, page))
            .done(function (data) {
                addUserData(data);
            })
            .fail(function () {
                addUserData();
            });
    }      
});