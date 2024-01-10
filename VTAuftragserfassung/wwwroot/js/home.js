

// Global Variables
let positionNr = 0;

// Format
function formatToCurrency(value) {
    return (value.toFixed(2) + '€').replace('.', ',');
}


function checkboxCheckedToInt(checked) {
    return checked ? "1" : "0";
}

// Assignment List

function toggleAssignmentDetails(ele) {
    let openEle = ele.parentElement.querySelector('.assignmentListRowWrapper.show');
    if (openEle) { openEle.classList.remove('show') };
    if (ele != openEle) { ele.classList.add('show') };
}


// Assignment Form

function openAssignmentForm() {
    removeOldAssignmentForm();
    let modalDiv = document.createElement('div');
    modalDiv.classList.add('assignmentModalContainer');
    modalDiv.innerHTML = backendRequestGET("/Home/NewAssignment");
    let main = document.querySelector('main');
    if (main) {
        main.parentNode.insertBefore(modalDiv, main.nextSibling);
    }
}

function removeOldAssignmentForm() {
    let oldAssignment = document.getElementsByClassName('assignmentModalContainer');
    if (oldAssignment.length > 0) {
        oldAssignment[0].remove();
        positionNr = 0;
    }
}

function saveNewAssignment() {
    let fk_customer = document.querySelector(`[data-name="assigmentCustomerFK"]`).value;
    if (!(validationAddBorder(document.getElementById('customerSearchBar'), fk_customer !== ''))) {
        alert('Bitte Kunden auswählen oder anlegen');
        return;
    }
    let assignmentBonus = document.querySelector(`[data-name="hasBonusCheckbox"]`).checked;
    let assignmentData = document.querySelectorAll('[property-name="assignmentData"]');
    let positionsListData = document.querySelectorAll('[property-name="positionsListData"]');

    let positionList = [];
    let assignmentViewObj = {
        PositionenVM: positionList, Auftrag: { FK_Kunde: fk_customer, SummeAuftrag: 0.00 }
    }

    positionsListData.forEach((obj, idx) => {
        let posDataSet = obj.querySelectorAll('[property-name="positionData"]');
        let artDataSet = obj.querySelectorAll('[property-name="articleData"]');
        let positionViewObj = { Position: {}, Artikel: {} };

        posDataSet.forEach((obj2, idx2) => {
            positionViewObj.Position[obj2.getAttribute('name')] = obj2.value;
        });

        artDataSet.forEach((obj3, idx3) => {
            positionViewObj.Artikel[obj3.getAttribute('name')] = obj3.value;
        });

        positionList.push(positionViewObj);        
    });

    assignmentData.forEach((obj4, idx4) => {
        assignmentViewObj.Auftrag[obj4.getAttribute('name')] = obj4.value;
    });

    assignmentViewObj.Auftrag.HatZugabe = checkboxCheckedToInt(assignmentBonus);
    backendRequestPOST("/Home/CreateNewAssignment/", assignmentViewObj);

    closeNewAssignment();
}


function closeNewAssignment() {
    let modalDiv = document.getElementsByClassName('assignmentModalContainer');
    modalDiv[0].remove();
    positionNr = 0;
}

// Assignment PositionRow Amount

function changePositionAmount(amountField, articlePrice, positionNumber) {
    let amount = parseInt(amountField.value);
    let calculatedPositionSum = amount * articlePrice;
    if (isNaN(amount) || amount <= 0) {
        amount = 1;
        amountField.value = 1;
    }
    updatePositionSum(positionNumber, calculatedPositionSum);
    updateAssignmentSum();
    amountField.setAttribute('value', amount);
}

function incrementPositionAmount(amountField, articlePrice, positionNumber) {
    amountField.value++;
    changePositionAmount(amountField, articlePrice, positionNumber);
}

function decrementPositionAmount(amountField, articlePrice, positionNumber) {
    if (amountField.value > 1) {
        amountField.value--;
        changePositionAmount(amountField, articlePrice, positionNumber);
    }
}

function updatePositionSum(positionNumber, positionSum) {
    let hiddenPositionSumElement = document.querySelector(`[data-row-id="hidden${positionNumber}"]`);
    let positionSumElement = document.querySelector(`[data-row-id="${positionNumber}"]`);
    positionSumElement.innerHTML = formatToCurrency(positionSum);
    hiddenPositionSumElement.setAttribute('value', positionSum);
}

function updateAssignmentSum() {
    let hiddenPositionSumElements = document.querySelectorAll('[property-name="positionData"][name="SummePosition"]');
    let hiddenAssignmentSumElement = document.querySelector('[data-row-id="hiddenAssignmentSum"]');
    let assignmentSumElement = document.querySelector('[data-row-id="assignmentSum"]');
    let calculatedAssignmentSum = 0.00;

    hiddenPositionSumElements.forEach((obj, idx) => {
        calculatedAssignmentSum += parseFloat(obj.value);
    });
    hiddenAssignmentSumElement.value = calculatedAssignmentSum;
    assignmentSumElement.innerHTML = formatToCurrency(calculatedAssignmentSum);
}

function removePosition(element) {
    element.remove();
    updateAssignmentSum();
}

// Customer Form

function openCustomerForm(btn) {
    btn.text = "Abbrechen";
    btn.setAttribute("onclick", "closeCustomerForm(this);");
    document.getElementById("customerSearch").style.display = "none";
    let targetElement = document.getElementById('selectedCustomer');
    let customerForm = backendRequestGET("/Home/AddCustomerForm/")
    let shareholderPartial = backendRequestGET("/Home/ShareholderFormPartial/");
    targetElement.innerHTML = customerForm;
    document.getElementById("selectedShareholder").innerHTML = shareholderPartial;
}


function closeCustomerForm(btn) {
    btn.text = "Kunden anlegen";
    btn.setAttribute("onclick", "openCustomerForm(this)");
    document.getElementById("customerSearch").style.display = "inline-block";
    document.getElementById('selectedCustomer').innerHTML = '';
    document.getElementById('selectedShareholder').innerHTML = '';
}

function inputValidationCheck(formIds) {
    let inputFields = [];
    let selectFields = [];
    let checkBoxes = [];
    let success = true;

    for (const formId of formIds) {
        let form = document.getElementById(formId);
        inputFields.push(...form.querySelectorAll('input[required]'));
        selectFields.push(...form.querySelectorAll('select[required]'));
        checkBoxes.push(...form.querySelectorAll('input[type="checkbox"][required]'));
    }

    for (const element of inputFields) {
        success = validationAddBorder(element, element.value.trim() !== '') ? success : false;
    }
    for (const element of selectFields) {
        success = validationAddBorder(element, element.value !== '' && element.value !== '0') ? success : false;
    }
    for (const element of checkBoxes) {
        success = validationAddBorder(element, element.checked) ? success : false;
    }

    if (!success) {
        alert('Bitte füllen Sie alle Pflichtfelder aus.');
    }
    return success;
}

function validationAddBorder(element, check) {
    check ? element.classList.remove('error-border') : element.classList.add('error-border');
    return check;
}

function saveNewCustomer() {
    if (!(inputValidationCheck(['customerForm', 'shareholderForm']))) {
        return;
    }

    let customerData = document.querySelectorAll('[property-name="customerData"]');
    let isWorkshopData = document.querySelector(`[data-name="isWorkshopCheckbox"]`).checked;
    let isSaleData = document.querySelector(`[data-name="isSaleCheckbox"]`).checked;
    let customer = {};
    customerData.forEach((obj, idx) => {
        customer[obj.getAttribute('name')] = obj.value;
    });
    customer.IstWerkstatt = checkboxCheckedToInt(isWorkshopData);
    customer.IstHandel = checkboxCheckedToInt(isSaleData);
    let customerPK = parseInt(backendRequestPOST("/Home/CreateNewCustomer/", customer));
    closeCustomerForm(document.getElementById("btnAddCustomer"));
    selectedCustomer(customerPK, "selectedCustomer");
}


// Search

function search(ele, searchTerm, model, backendMethod) {
    let modelList = model;
    let propertyArray = [];
    let resultList = [];
    let searchResultDiv = document.getElementsByClassName('searchResult');
    if (searchResultDiv.length > 0) {
        searchResultDiv[0].remove();
    }
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
        searchResultDiv.innerHTML = backendRequestPOST(backendMethod, resultList);
    } else {
        searchResultDiv.innerHTML = '';
    }
}

function searchResultSelected(modelPK, targetElementId) {
    if (targetElementId == "selectedArticle") {
        selectedArticle(modelPK, targetElementId);
    }
    else if (targetElementId == "selectedCustomer") {
        selectedCustomer(modelPK, targetElementId);
    }
    
    document.getElementById("searchTable").remove();
    document.getElementsByClassName("searchResult")[0].remove();
    let searchBarElement = document.getElementsByClassName("searchBar");
    for (const element of searchBarElement) {
        element.value = '';
    }
}


function selectedCustomer(modelPK, targetElementId) {
    let targetPartial = backendRequestGET("/Home/AddCustomerDetailsPartial/" + modelPK);
    let targetElement = document.getElementById(targetElementId);
    targetElement.innerHTML = targetPartial;

    document.querySelector(`[data-name="assigmentCustomerFK"]`).value = modelPK;

    let shareholderFK = targetElement.querySelector(`[data-name="shareholderFK"]`).dataset.fk_shareholder;
    let shareholderPartial = backendRequestGET("/Home/ShareholderDetailsPartial/" + shareholderFK);
    document.getElementById("selectedShareholder").innerHTML = shareholderPartial;
}

function selectedArticle(modelPK, targetElementId) {
    positionNr++;
    let targetPartial = backendRequestGET("/Home/AddPositionListRowFormPartial/" + modelPK + "?positionNr=" + positionNr);
    let targetElement = document.getElementById(targetElementId);
    targetElement.innerHTML = targetElement.innerHTML + targetPartial;
    updateAssignmentSum();
}





