
// Assignments
function toggleAssignmentDetails(ele) {
    let openEle = ele.parentElement.querySelector('.assignmentListRowWrapper.show');
    if (openEle) { openEle.classList.remove('show'); }
    if (ele != openEle) { ele.classList.add('show'); }
}

function newAssignment() {
    let modalDiv = document.createElement('div');
    modalDiv.ClassName = 'assignmentModal';
    modalDiv.innerHTML = backendRequest("GET", "/Home/NewAssignment");
    let main = document.querySelector('main');
    if (main) {
        main.parentNode.insertBefore(modalDiv, main.nextSibling);
    }
}

function search(searchterm, model) {

    let modelList = Html.Raw(System.Text.Json.JsonSerializer.Serialize(model));
    let propertyArray = [];
    let resultList = [];
    //for each object in index
    for (const element of modelList) {

        //does any of the objects property values.tostring contains searchterm?
        //make array of propertyvalues
        //add object to resultlist

        propertyArray.push(Object.values(element));
        if (JSON.stringify(propertyarray).toLowerCase().includes(searchterm.trim().toLowerCase()) && searchterm.length > 0) {
            resultList.push(element)
        }
        propertyArray = [];
    }
    //if resultlist any
    //show resultlist
    //emptyresultlist
    if (resultList.length > 0) {
        document.getElementById("searchresult").innerHTML = '<table id="searchTable" class="table table-striped table-bordered zero-configuration dataTable" role="grid">'
            + '<thead><tr><th>Result</th></tr></thead>'
            + '<tbody id="resulttblbody"></tbody>'
            + '</table>';
        for (const element of resultList) {
            document.getElementById("resulttblbody").innerHTML += `<tr><td><a name="${Object.values(element)[0]}" class="btn btn-outline-secondary"
                                    onMouseOut="this.style.color='#000'"
                                    onMouseOver="this.style.color='#F0F'"
                                    onclick="searchResultSelected(this.name)">${Object.values(element)[0]} ${Object.values(element)[1]} ${Object.values(element)[2]}</a></td></tr>`
        }
    }
    else {
        document.getElementById("searchresult").innerHTML = '';
    }
}