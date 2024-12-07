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

// Add Subtask

function removeTask(button) {
    const row = button.closest('tr');
    row.remove();

    reindexTasks();
}

function addSubTask() {
    const taskNameInput = document.getElementById('NewTaskName');
    const taskContainer = document.getElementById('SubTaskContainer');
    const subTaskError = document.getElementById('SubTaskError');
    const taskName = taskNameInput.value.trim();

    subTaskError.textContent = '';

    if (taskName === '') {
        subTaskError.textContent = 'Task name cannot be empty!';
        return;
    }

    if (validateTask(taskName, taskContainer)) {
        subTaskError.textContent = 'Task with the same name already exists!';
        return;
    }

    const row = document.createElement('tr');
    row.innerHTML = `
        <td>
            <input type="checkbox" name="Tasks[].IsCompleted" value="false" />
            <input type="hidden" name="Tasks[].Id" value="0" />
            <input type="hidden" name="Tasks[].Name" value="${taskName}" />
            ${taskName}
        </td>
        <td>
            <button type="button" class="btn btn-danger btn-sm" onclick="removeTask(this)">Delete</button>
        </td>
    `;

    taskContainer.appendChild(row);
    taskNameInput.value = '';

    reindexTasks();
}

function validateTask(taskName, taskContainer) {
    const existingTasks = Array.from(taskContainer.querySelectorAll('input[name$=".Name"]'));
    return existingTasks.some(task => task.value === taskName);
}

function reindexTasks() {
    const taskContainer = document.getElementById('SubTaskContainer');
    const rows = Array.from(taskContainer.querySelectorAll('tr'));

    rows.forEach((row, index) => {
        const checkbox = row.querySelector('input[type="checkbox"]');
        const hiddenId = row.querySelector('input[name$=".Id"]');
        const hiddenName = row.querySelector('input[name$=".Name"]');

        if (checkbox) checkbox.name = `Tasks[${index}].IsCompleted`;
        if (hiddenId) hiddenId.name = `Tasks[${index}].Id`;
        if (hiddenName) hiddenName.name = `Tasks[${index}].Name`;
    });
}