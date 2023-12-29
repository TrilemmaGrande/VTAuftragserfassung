
// Assignments
function toggleAssignmentDetails(ele) {
    let openEle = ele.parentElement.querySelector('.assignmentListRowWrapper.show');
    if (openEle) { openEle.classList.remove('show'); }
    if (ele != openEle) { ele.classList.add('show'); }
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

function search(searchTerm, model) {
    let modelList = model;
    let propertyArray = [];
    let resultList = [];

    for (const element of modelList) {
        propertyArray.push(Object.values(element));

        if (JSON.stringify(propertyArray).toLowerCase().includes(searchTerm.trim().toLowerCase()) && searchTerm.length > 0) {
            resultList.push(element);
        }

        propertyArray = [];
    }

    if (resultList.length > 0) {
        document.getElementById("searchResult").innerHTML = '<div id="searchTable">'
            + '<div class="gridContainer" id="resultTblBody"></div>'
            + '</div>';

        for (const article of resultList) {
            document.getElementById("resultTblBody").innerHTML +=
                `<div><a href="javascript:void(0);" data-name="${Object.values(article)[0]}" class="resultTblRow"
                                    onMouseOut="this.style.color='#000'"
                                    onMouseOver="this.style.color='#165b9e'"
                                    onclick="searchResultSelected(this.getAttribute('data-name'))">
                                     ${Object.values(article)[1]} ${Object.values(article)[2]}
                                     ${Object.values(article)[3]}  ${Object.values(article)[4]}  ${Object.values(article)[5]}
                                    </a></div>`;
        }
    } else {
        document.getElementById("searchResult").innerHTML = '';
    }
}
function searchResultSelected(articlePK) {
    document.getElementById("selectedItem").innerHTML += backendRequest("GET", "/Home/AddPositionListRowFormPartial/" + articlePK);
    document.getElementById("searchTable").remove();
    document.getElementById("searchBar").value = '';
}
