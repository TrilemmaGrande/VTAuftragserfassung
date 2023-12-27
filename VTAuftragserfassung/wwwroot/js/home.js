
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
