// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//$(document).ready(async function (id) {
document.addEventListener("DOMContentLoaded", async function () {

    $("#favouriteContainer").hide();

    //function setCookie(name, value, days) {
    //    const date = new Date();
    //    date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000)); 
    //    const expires = "expires=" + date.toUTCString();
    //    document.cookie = `${name}=${value}; ${expires}; path=/`;
    //}

    //function getCookie(name) {
    //    const cookies = document.cookie.split("; ");
    //    for (let cookie of cookies) {
    //        const [key, value] = cookie.split("="); 
    //        if (key === name) {
    //            return decodeURIComponent(value); 
    //        }
    //    }
    //    return null; 
    //}

    function isNullOrWhitespace(input) {

        if (typeof input === 'undefined' || input == null) return true;

        return input.replace(/\s/g, '').length < 1;
    }

    //function deleteCookie(name) {
    //    document.cookie = name + "=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
    //}

    function loadRowData(id) {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: `/Home/GetRestaurant/${id}`,
                method: "GET",
                success: function (data) {
                    resolve(data); 
                },
                error: function (xhr, status, error) {
                    reject(error); 
                }
            });
        });
    }

    async function loadFavsRowData(id) {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: `/Home/GetFavsList/${id}`,
                method: "GET",
                success: function (data) {
                    resolve(data);
                },
                error: function (xhr, status, error) {
                    reject(error);
                }
            });
        });
    }

    function createFavsList() {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: `/Home/CreateFavsList`,
                method: "GET",
                success: function (data) {
                    resolve(data);
                },
                error: function (xhr, status, error) {
                    reject(error);
                }
            });
        });
    }

    function getFormData($form) {
        var unindexed_array = $form.serializeArray();
        var indexed_array = {};

        $.map(unindexed_array, function (n, i) {
            indexed_array[n['name']] = n['value'];
        });

        return indexed_array;
    }


    function addFav() {
        return new Promise((resolve, reject) => {
            const rating = $("#rating").val();
            const notes = $("#notes").val();
            const guid = $("#idguid").text();

            var obj = {
                restaurantId: guid,
                rating: parseInt(rating),
                notes: notes
            }

            $.ajax({
                url: `/Home/AddFavs`,
                method: "POST",
                contentType: "application/json", 
                data: JSON.stringify(obj),
                success: function (data) {
                    resolve(data);
                },
                error: function (xhr, status, error) {
                    reject(error);
                }
            });
        });
    }

    function deleteFav(favId, restId) {
        $("#rating").val(""); 
        $("#notes").val("");
        $("#saveContainer").hide();
 
        return new Promise((resolve, reject) => {
            var obj = {
                favId: favId,
                restId: restId
            }

            $.ajax({
                url: `/Home/DeleteFav`,
                method: "POST",
                contentType: "application/json",
                data: JSON.stringify(obj),
                success: function (data) {
                    resolve(data);
                },
                error: function (xhr, status, error) {
                    reject(error);
                }
            });
        });
    }

    $("#submitter").on("submit", async function (event) {
        event.preventDefault(); 
        const data = await addFav();
        await refreshTable();
        $("#favouriteContainer").show();
    });
    async function refreshTable() {
        const favValue = $("#favidguid").text();
        const favsData = await loadFavsRowData(favValue);
        $("#favsTable tbody").empty();
        $("#favsTable tbody").hide();
        $("#favsTable thead").hide();
        favsData.favourites.forEach(function (fav) {
            $("#favsTable tbody").append(`
            <tr data-id="${fav.id}">
                <td>${fav.restaurantId}</td>
                <td>${fav.rating}</td>
                <td>${fav.notes}</td>
            </tr>
            `)
            $("#favsTable tbody").show();;
            $("#favsTable thead").show(); 
        });
    }

    $(document).on("change", "#fav",async function () {
        checkChangedForEdit();
    });

    async function checkChangedForEdit() {
        if ($("#fav").is(":checked")) {
            const value = $("#idguid").text();
            const favValue = $("#favidguid").text();

            if (isNullOrWhitespace(favValue)) {
                const data = await createFavsList();
                console.log("Data loaded for ID:", data);

                $("#favidguid").html(data.favId);
                $("#myDiv").hide();
            }
            $("#saveContainer").show();
            console.log("Favorite selected:", value);
            /*setCookie(value, "Favourite", 30);*/
        }
        else {
            $("#saveContainer").hide();
            /*deleteCookie(id);*/
        }
    }

    $(document).on("change", "#notfav",async function () {
        if ($(this).is(":checked")) {
            $("#saveContainer").hide();
            /*deleteCookie(id);*/

            const favValue = $("#favidguid").text();
            const value = $("#idguid").text();
            const data = await deleteFav(favValue, value); 
            await refreshTable();
        }
        else {
            const value = $("#idguid").text();
            console.log("Favorite selected:", value);
            /*setCookie(value, "Favourite", 30);*/
            $("#saveContainer").show();
        }
    });

    $(document).on("click", "table tbody tr", async function () {
        const id = $(this).data("id"); 
        console.log("Row ID clicked:", id);
        $("#saveContainer").show();
        $("#favouriteContainer").show();
        $("#rating").val("");
        $("#notes").val("");
        checkChangedForEdit();

        try {
            const data = await loadRowData(id); 
            console.log("Data loaded for ID:", id, data);

            $("#idguid").html(data.selectedRestaurant.id);
            $("#name").html(data.selectedRestaurant.name);
            $("#cuisine").html(data.selectedRestaurant.cuisine);
            $("#streetaddress").html(data.selectedRestaurant.streetaddress);
            $("#city").html(data.selectedRestaurant.city);
            $("#state").html(data.selectedRestaurant.state);
            $("#zip").html(data.selectedRestaurant.zip);
            $("#phonenumber").html(data.selectedRestaurant.phonenumber);
            $("#glutenfree").html(String(data.selectedRestaurant.dietaryrestrictions.glutenfree));
            $("#dairyfree").html(String(data.selectedRestaurant.dietaryrestrictions.dairyfree));
            $("#pricerange").html(String(data.selectedRestaurant.pricerange));
            $("#delivery").html(String(data.selectedRestaurant.delivery));

            $("#recordContainer").show();
            $("#radios").show();
            $("#saveContainer").show();
            $("#fav").prop("checked", true);
            await refreshTable();

        } catch (error) {
            console.error("Error fetching row data:", error);
        }
    });
});
