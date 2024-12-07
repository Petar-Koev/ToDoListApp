// Add ToDo
function addTask() {
    const taskNameInput = document.getElementById('TaskName');
    const taskContainer = document.getElementById('TaskContainer');

    if (taskNameInput.value.trim() === '') {
        alert('Task name cannot be empty!');
        return;
    }

    const li = document.createElement('li');
    li.textContent = taskNameInput.value;

    const removeButton = document.createElement('button');
    removeButton.textContent = 'X';
    removeButton.className = 'remove-button';
    removeButton.onclick = () => li.remove();

    li.appendChild(removeButton);
    taskContainer.appendChild(li);

    const hiddenInput = document.createElement('input');
    hiddenInput.type = 'hidden';
    hiddenInput.name = 'Tasks';
    hiddenInput.value = taskNameInput.value;

    li.appendChild(hiddenInput);
    taskNameInput.value = '';
}

// Index ToDo
function toggleTasks(todoId) {
    var tasksRow = document.getElementById(`tasks-${todoId}`);
    tasksRow.style.display = tasksRow.style.display === "none" ? "table-row" : "none";
}