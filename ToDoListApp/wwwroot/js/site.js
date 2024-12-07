// Add ToDo
function validateTask(taskName, taskContainer) {
    const existingTasks = Array.from(taskContainer.querySelectorAll('input[name^="Tasks["]')); 
    return existingTasks.some(task => task.value === taskName);
}

function createRemoveButton(li) {
    const removeButton = document.createElement('button');
    removeButton.textContent = 'X';
    removeButton.className = 'remove-button';
    removeButton.onclick = () => li.remove();
    return removeButton;
}

function createTaskElement(taskName) {
    const li = document.createElement('li');

    const taskContent = document.createElement('div');
    taskContent.className = 'task-content'; 

    const taskNameSpan = document.createElement('span');
    taskNameSpan.textContent = taskName;

    taskContent.appendChild(createRemoveButton(li));
    taskContent.appendChild(taskNameSpan);

    li.appendChild(taskContent);

    return li;
}

function addTask() {
    const taskNameInput = document.getElementById('TaskName');
    const taskContainer = document.getElementById('TaskContainer');
    const taskError = document.getElementById('TaskError');
    const taskName = taskNameInput.value.trim();

    taskError.textContent = '';

    if (taskName === '') {
        taskError.textContent = 'Task name cannot be empty!';
        return;
    }

    if (validateTask(taskName, taskContainer)) {
        taskError.textContent = 'Task with the same name already exists!';
        return;
    }

    const newTaskElement = createTaskElement(taskName);

    const hiddenInput = document.createElement('input');
    hiddenInput.type = 'hidden';
    hiddenInput.name = `Tasks[${taskContainer.children.length}].Name`;
    hiddenInput.value = taskName;
   
    newTaskElement.appendChild(hiddenInput);
    taskContainer.appendChild(newTaskElement);

    taskNameInput.value = '';
}

// Index ToDo
function toggleTasks(todoId) {
    var tasksRow = document.getElementById(`tasks-${todoId}`);
    tasksRow.style.display = tasksRow.style.display === "none" ? "table-row" : "none";
}