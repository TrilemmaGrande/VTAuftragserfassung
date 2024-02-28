// Global Variables
let _positionNr = 0;
let _maxPage = 1;
let searchTimeout;

// Format
function formatToCurrency(value) {
    return (value.toFixed(2) + '€').replace('.', ',');
}

function checkboxCheckedToInt(checked) {
    return checked ? "1" : "0";
}

// Main

function setMainPage(view) {
    let main = document.querySelector('main');
    if (main && view) {
        main.innerHTML = '';
        main.appendChild(view);
    }
}

// Pagination

function calcMaxPage(paginationModel) {
    if (paginationModel.page == null || paginationModel.linesPerPage == null) {
        return;
    }
    let assignmentCount = backendRequestGET("/home/GetAssignmentsCount/")
    _maxPage = Math.ceil(assignmentCount / paginationModel.linesPerPage);
}

function getPagination(userId, viewName) {
    let paginationModel = { page: 1, linesPerPage: 20 };
    let localStoragePage = localStorage.getItem(userId + viewName + 'paginationSettings');
    if (localStoragePage != null) {
        paginationModel = JSON.parse(localStoragePage);
    }
    else {
        setPaginationToDefault(userId, viewName);
    }
    return paginationModel;
}

function setPagination(userId, viewName, pagination) {
    if (pagination == null) {
        setPaginationToDefault(userId, viewName);
    }
    else {
        localStorage.setItem(userId + viewName + 'paginationSettings', JSON.stringify(pagination));
    }
}

function setPaginationToDefault(userId, viewName) {
    localStorage.setItem(userId + viewName + 'paginationSettings', JSON.stringify({ page: 1, linesPerPage: 20 }));
}

// Assignment List

function changePageAssignmentList(page, linesPerPage) {
    let userId = backendRequestGET("/Home/GetUserId");
    let paginationModel = getPagination(userId, 'assignment');
    paginationModel.page = page ?? paginationModel.page;
    paginationModel.linesPerPage = linesPerPage ?? paginationModel.linesPerPage;
    calcMaxPage(paginationModel);
    if (paginationModel.page > _maxPage) {
        paginationModel.page = _maxPage;
    }
    if (paginationModel.page < 1) {
        paginationModel.page = 1;
    }
    setAssignmentList(paginationModel);
    setPagination(userId, 'assignment', paginationModel);
}

function openAssignmentList() {
    let userId = backendRequestGET("/Home/GetUserId")
    let paginationModel = getPagination(userId, 'assignment');
    calcMaxPage(paginationModel);
    if (paginationModel.page == null || paginationModel.linesPerPage == null
        || paginationModel.page < 1 || paginationModel.page > _maxPage) {
        setPaginationToDefault(userId, 'assignment');
        paginationModel = getPagination(userId, 'assignment');
    }
    setAssignmentList(paginationModel);
}

function setAssignmentList(paginationModel) {
    let view = document.createElement('div')
    view.innerHTML = (backendRequestPOST("/Home/AssignmentsPartial/", paginationModel) ?? "");
    let paginationEle = view.querySelector('#paginationMenu');
    if (paginationEle == null) {
        return null;
    }
    paginationEle.innerHTML = (backendRequestPOST("/Home/PaginationMenuPartial", paginationModel));
    setMainPage(view);
    return 1;
}
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
    };
}

function removeOldAssignmentForm() {
    let oldAssignment = document.getElementsByClassName('assignmentModalContainer');
    if (oldAssignment.length > 0) {
        oldAssignment[0].remove();
        _positionNr = 0;
    }
}

function saveNewAssignment() {
    let fk_customer = document.querySelector(`[data-name="assignmentCustomerFK"]`).value;
    if (!(validationAddBorder(document.getElementById('customerSearchBar'), fk_customer !== ''))) {
        alert('Bitte Kunden auswählen oder anlegen');
        return;
    }
    let assignmentBonus = document.querySelector(`[data-name="hasBonusCheckbox"]`).checked;
    let assignmentData = document.querySelectorAll('[property-name="assignmentData"]');
    let positionsListData = document.querySelectorAll('[property-name="positionsListData"]');

    let positionList = [];
    let assignmentViewObj = {
        PositionenVM: positionList,
        Auftrag: { FK_Kunde: fk_customer, SummeAuftrag: 0.00, Hinweis: "", HatZugabe: 0}
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

    if (!assignmentViewObj.Auftrag.SummeAuftrag) {
        assignmentViewObj.Auftrag.SummeAuftrag = '0';
    }

    assignmentViewObj.Auftrag.HatZugabe = checkboxCheckedToInt(assignmentBonus);
    backendRequestPOST("/Home/CreateNewAssignment/", assignmentViewObj);

    closeNewAssignment();
    changePageAssignmentList(1, null);
}

function cancelAssignment(modelPK) {
    let assignmentStatus = "Storniert";
    backendRequestPOST("/Home/UpdateAssignmentStatus/" + modelPK + "?assignmentStatus=" + assignmentStatus);
}

function closeNewAssignment() {
    let modalDiv = document.getElementsByClassName('assignmentModalContainer');
    modalDiv[0].remove();
    _positionNr = 0;
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
    document.getElementById("customerSearch").style.display = "none";
    let targetElement = document.getElementById('selectedCustomer');
    let customerForm = backendRequestGET("/Home/AddCustomerFormPartial/")
    let shareholderPartial = backendRequestGET("/Home/AddShareholderFormPartial/");
    targetElement.innerHTML = customerForm;
    document.getElementById("selectedShareholder").innerHTML = shareholderPartial;


    closeSearchTable();
}

function closeCustomerForm(btn) {
    document.getElementById("customerSearch").style.display = "block";
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
    if (!(inputValidationCheck(['selectedCustomer', 'selectedShareholder']))) {
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
    let customerPK = backendRequestPOST("/Home/CreateNewCustomer/", customer);
    closeCustomerForm(document.getElementById("btnAddCustomer"));
    customer.pK_Kunde = customerPK;
    setSelectedCustomerPartialByCustomer(customer, "selectedCustomer");
}

// Search

function search(ele, searchTerm, backendMethod) {
    clearTimeout(searchTimeout);
    let timeout = 0;
    if (searchTerm.length < 2 && searchTerm.length > 0) {
        timeout = 500;
    }

    let searchResultDiv = document.getElementsByClassName('searchResult');
    searchTimeout = setTimeout(function () {
        if (searchResultDiv.length > 0 || searchTerm.length == 0) {
            searchResultDiv[0].remove();
        }
        if (!searchTimeout || searchTerm.length == 0) {
            return;
        }

        searchResultDiv = document.createElement('div');
        searchResultDiv.classList.add('searchResult');
        ele.after(searchResultDiv);
        let model = backendRequestGET(backendMethod + searchTerm);
        model = Array.from(JSON.parse(model));
        searchResultDiv.innerHTML = searchResultController(backendMethod, model);
    }, timeout);
}

function searchResultController(backendMethod, model) {
    if (backendMethod == "/Home/SearchResultPartialArticle/") {
        return searchResultArticle(model);
    }
    else if (backendMethod == "/Home/SearchResultPartialCustomer/") {
        return searchResultCustomer(model);
    }
}
function searchResultCustomer(model) {
    let div = '';

    if (model && model.length > 0) {
        div = `
        <div id="searchTable">
            <div class="headerListWrapper">
                <div class="gridListContentContainer gridListContent5">
                    <div>Kd.-Nr.</div>
                    <div>Vorname</div>
                    <div>Nachname</div>
                    <div>Firma</div>
                    <div>Ort</div>
                </div>
            </div>
            <div style="overflow:auto;">
                <div class="gridContainer" id="resultTblBody">`;
        model.forEach(item => {
            div += `
            <a class="gridListContentContainer gridListContentSearchResult"
        onMouseOut = "this.style.color='#000'"
        onMouseOver = "this.style.color='#165b9e'"
        onclick = "searchResultSelected(${item.pK_Kunde}, 'selectedCustomer')" >
                    <div>${item.kundennummer}</div>
                    <div>${item.vorname}</div>
                    <div>${item.nachname}</div>
                    <div>${item.firma}</div>
                    <div>${item.ort}</div>
                </a > `;
        });

        div += `
                </div >
            </div >
        </div >`;
    }

    return div;
}
function searchResultArticle(model) {
    let div = '';
    if (model && model.length > 0) {
        div += `
        <div id="searchTable" >
            <div class="headerListWrapper">
                <div class="gridListContentContainer gridListContent5">
                    <div>Art. Nr.</div>
                    <div>Bezeichnung</div>
                    <div>Zusatzbezeichnung</div>
                    <div>Einzelpreis Netto</div>
                    <div>Verpackungseinheit</div>
                </div>
            </div>
            <div style="overflow:auto;">
               <div class="gridContainer" id="resultTblBody">`;
        model.forEach(item => {
            div += `
                     <a class="gridListContentContainer gridListContentSearchResult"
                       onMouseOut="this.style.color='#000'"
                       onMouseOver="this.style.color='#165b9e'"
                       onclick="searchResultSelected(${item.pK_Artikel}, 'selectedArticle')">
                        <div>${item.artikelnummer}</div>
                        <div>${item.bezeichnung1}</div>
                        <div>${item.bezeichnung2}</div>
                        <div>${formatToCurrency(item.preis)}</div>
                        <div>${item.verpackungseinheit}</div>
                    </a>
                      </a>`;
        });

        div += `
              </div>
            </div>
        </div>
        `
    }
    return div;
}
function searchResultSelected(modelPK, targetElementId) {
    if (targetElementId == "selectedArticle") {
        setSelectedArticlePartialByPk(modelPK, targetElementId);
    }
    else if (targetElementId == "selectedCustomer") {
        setSelectedCustomerPartialByPk(modelPK, targetElementId);
    }
    closeSearchTable();
}

function closeSearchTable() {
    let searchTable = document.getElementById("searchTable")
    if (searchTable) {
        searchTable.remove();
    }        
    let searchResult = document.getElementsByClassName("searchResult")[0]
    if (searchResult) {
        searchResult.remove();
    }        
    let searchBarElement = document.getElementsByClassName("searchBar");
    for (const element of searchBarElement) {
        element.value = '';
    }
}

function setSelectedCustomerPartialByPk(modelPK, targetElementId) {
    let targetPartial = backendRequestGET("/Home/AddCustomerDetailsPartial/" + modelPK);
    let targetElement = document.getElementById(targetElementId);
    targetElement.innerHTML = targetPartial;

    document.querySelector('[data-name="assignmentCustomerFK"]').value = modelPK;
    let shareholderFK = targetElement.querySelector(`[data-name="shareholderFK"]`).dataset.fk_shareholder;
    
    setShareholderPartialByPk(shareholderFK);
}

function setSelectedCustomerPartialByCustomer(customer, targetElementId) {
    if (!customer && !targetElementId) { return; }
    let targetPartial = backendRequestPOST("/Home/AddCreatedCustomerDetailsPartial/", customer);
    let targetElement = document.getElementById(targetElementId);
    targetElement.innerHTML = targetPartial;

    document.querySelector('[data-name="assignmentCustomerFK"]').value = customer.pK_Kunde;
    let shareholderFK = customer.FK_Gesellschafter;
    setShareholderPartialByPk(shareholderFK);
}

function setShareholderPartialByPk(modelPK) {
    let shareholderPartial = backendRequestGET("/Home/ShareholderDetailsPartial/" + modelPK);
    document.getElementById("selectedShareholder").innerHTML = shareholderPartial;
}

function setSelectedArticlePartialByPk(modelPK, targetElementId) {
    let incrementArticleButton = document.getElementById("increment_" + modelPK)
    if (incrementArticleButton) {
        incrementArticleButton.click();
        return;
    }
    _positionNr++;
    let targetPartial = backendRequestGET("/Home/AddPositionListRowFormPartial/" + modelPK + "?positionNr=" + _positionNr);
    let targetElement = document.getElementById(targetElementId);
    targetElement.innerHTML = targetElement.innerHTML + targetPartial;
    updateAssignmentSum();
}

function IntToBool(i) {
    return i !== 0;
}