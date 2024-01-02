
// Assignments
function toggleAssignmentDetails(ele) {
    let openEle = ele.parentElement.querySelector('.assignmentListRowWrapper.show');
    if (openEle) { openEle.classList.remove('show'); }
    if (ele != openEle) { ele.classList.add('show'); }
}

function openCustomerForm() {
    let targetElement = document.getElementById('selectedCustomer');
    let customerForm = backendRequest("GET", "/Home/AddCustomerForm/")
    targetElement.innerHTML = customerForm;
}

function newAssignment() {
    let modalDiv = document.createElement('div');
    modalDiv.classList.add('assignmentModalContainer');
    modalDiv.innerHTML = backendRequest("GET", "/Home/NewAssignment");
    let main = document.querySelector('main');
    if (main) {
        main.parentNode.insertBefore(modalDiv, main.nextSibling);
    }
}

function addNewAssignment() {
}

function closeNewAssignment() {
    let modalDiv = document.getElementsByClassName('assignmentModalContainer');
    modalDiv[0].remove();
}

function search(ele, searchTerm, model, backendMethod) {
    let modelList = model;
    let propertyArray = [];
    let resultList = [];
    let searchResultDiv = document.getElementsByClassName('searchResult');
    if (searchResultDiv.length > 0) {
        searchResultDiv[0].remove();
    };
    for (const element of modelList) {
        propertyArray.push(Object.values(element));

        if (JSON.stringify(propertyArray).toLowerCase().includes(searchTerm.trim().toLowerCase()) && searchTerm.length > 0) {
            resultList.push(element);
        }

        propertyArray = [];
    }

    searchResultDiv = document.createElement('div');
    searchResultDiv.classList.add('searchResult');
    ele.after(searchResultDiv);
    if (resultList.length > 0) {
        searchResultDiv.innerHTML = backendRequest("POST", backendMethod, resultList);
    } else {
        searchResultDiv.innerHTML = '';
    }
}

function searchResultSelected(modelPK, targetElementId) {
    let targetPartial;
    if (targetElementId == "selectedArticle") {
        targetPartial = backendRequest("GET", "/Home/AddPositionListRowFormPartial/" + modelPK);
        document.getElementById(targetElementId).innerHTML += targetPartial;
    }
    else if (targetElementId == "selectedCustomer") {
        targetPartial = backendRequest("GET", "/Home/AddCustomerDetailsPartial/" + modelPK)
        document.getElementById(targetElementId).innerHTML = targetPartial;
    }
    document.getElementById("searchTable").remove();
    document.getElementsByClassName("searchResult")[0].remove();
    let searchBarElement = document.getElementsByClassName("searchBar");
    for (const element of searchBarElement) {
        element.value = '';
    }
}
