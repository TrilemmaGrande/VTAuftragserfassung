

// Global Variables
let positionNr = 0;

// Assignments
function toggleAssignmentDetails(ele) {
    let openEle = ele.parentElement.querySelector('.assignmentListRowWrapper.show');
    if (openEle) { openEle.classList.remove('show'); }
    if (ele != openEle) { ele.classList.add('show'); }
}

function openCustomerForm(btn) {
    btn.text = "Abbrechen";
    btn.setAttribute("onclick", "closeCustomerForm(this);");
    document.getElementById("customerSearch").style.display = "none";
    let targetElement = document.getElementById('selectedCustomer');
    let customerForm = backendRequest("GET", "/Home/AddCustomerForm/")
    let shareholderPartial = backendRequest("GET", "/Home/ShareholderFormPartial/");
    targetElement.innerHTML = customerForm;
    document.getElementById("selectedShareholder").innerHTML = shareholderPartial;
}


function closeCustomerForm(btn) {
    btn.text = "Kunden anlegen";
    btn.setAttribute("onclick", "openCustomerForm(this)");
    document.getElementById("customerSearch").style.display = "block";
    document.getElementById('selectedCustomer').innerHTML = '';
    document.getElementById('selectedShareholder').innerHTML = '';
}

function saveCustomer() {
    // saveCustomer!!
    document.querySelectorAll('customerData')
    // select Customer:
    /* selectedCustomer(   HIER MUSS NOCH DIE PK   ,"selectedCustomer")*/
    closeCustomerForm(document.getElementById("btnAddCustomer"));
}

function newAssignment() {
    let oldAssignment = document.getElementsByClassName('assignmentModalContainer');
    if (oldAssignment.length > 0) {
        oldAssignment[0].remove();
        positionNr = 0;
    }
    let modalDiv = document.createElement('div');
    modalDiv.classList.add('assignmentModalContainer');
    modalDiv.innerHTML = backendRequest("GET", "/Home/NewAssignment");
    let main = document.querySelector('main');
    if (main) {
        main.parentNode.insertBefore(modalDiv, main.nextSibling);
    }
}

function saveNewAssignment() {
    // get customer PK
    // get position data < article FK
    // get bonus checkbox data
    // get notice textarea data
    // make object
    // send to backend
    closeNewAssignment();
    // refresh assignment list
}

function closeNewAssignment() {
    let modalDiv = document.getElementsByClassName('assignmentModalContainer');
    modalDiv[0].remove();
    positionNr = 0;
}

function changePositionAmount(amountField, articlePrice, positionNumber) {
    let amount = amountField.value;
    if (isNaN(amount) || amount <= 0) {
        amount = 1;
        amountField.value = 1;
    }
    let positionSumElement = document.querySelector(`[data-row-id="${positionNumber}"]`);
    let calculatedSum = amount * articlePrice;
    positionSumElement.innerHTML = formatCurrency(calculatedSum);
}

function formatCurrency(value) {
    return (value.toFixed(2) + '€').replace('.',',');
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
    if (targetElementId == "selectedArticle") {
        selectedArticle(modelPK) 
    }
    else if (targetElementId == "selectedCustomer") {
        selectedCustomer(modelPK, targetElementId) 
    }
    document.getElementById("searchTable").remove();
    document.getElementsByClassName("searchResult")[0].remove();
    let searchBarElement = document.getElementsByClassName("searchBar");
    for (const element of searchBarElement) {
        element.value = '';
    }
}

function selectedArticle(modelPK, targetElementId) {
    positionNr++;
    let targetPartial = backendRequest("POST", "/Home/AddPositionListRowFormPartial/" + modelPK + "?positionNr=" + positionNr);
    let targetElement = document.getElementById(targetElementId)
    targetElement.innerHTML += targetPartial;
}

function selectedCustomer(modelPK, targetElementId) {
    let targetPartial = backendRequest("GET", "/Home/AddCustomerDetailsPartial/" + modelPK)
    let targetElement = document.getElementById(targetElementId)
    targetElement.innerHTML = targetPartial;

    let shareholderFK = targetElement.querySelector(`[data-name="shareholderFK"]`).dataset.fk_shareholder;
    let shareholderPartial = backendRequest("GET", "/Home/ShareholderDetailsPartial/" + shareholderFK);
    document.getElementById("selectedShareholder").innerHTML = shareholderPartial;
}


