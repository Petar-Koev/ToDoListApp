// Shared functions

function validateTask(taskName, taskContainer, selector) {
    const existingTasks = Array.from(taskContainer.querySelectorAll(selector));
    return existingTasks.some(task => task.value === taskName);
}

function validateTaskInput(taskName, taskContainer, errorElement, selector) {
    if (taskName === '') {
        errorElement.textContent = 'Task name cannot be empty!';
        return false;
    }

    if (validateTask(taskName, taskContainer, selector)) {
        errorElement.textContent = 'Task with the same name already exists!';
        return false;
    }

    if (taskName.length < 2) {
        errorElement.textContent = 'Task name must be at least 2 characters long!';
        return false;
    }

    if (taskName.length > 100) {
        errorElement.textContent = 'Task name cannot exceed 100 characters!';
        return false;
    }

    errorElement.textContent = ''; 
    return true;
}

// Add ToDo Subtask functionality in Create ToDo

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

    if (!validateTaskInput(taskName, taskContainer, taskError, 'input[name^="Tasks["]')) {
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

// Add ToDo Subtask functionality in Show ToDo Subtasks

function removeTask(button) {
    const row = button.closest('tr');
    row.remove();

    reindexTasks();
}

function toggleCheckboxValue(checkbox) {
    checkbox.value = checkbox.checked ? "true" : "false";
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

function addSubTask() {
    const taskNameInput = document.getElementById('NewTaskName');
    const taskContainer = document.getElementById('SubTaskContainer');
    const subTaskError = document.getElementById('SubTaskError');
    const taskName = taskNameInput.value.trim();

    if (!validateTaskInput(taskName, taskContainer, subTaskError, 'input[name$=".Name"]')) {
        return;
    }

    const row = document.createElement('tr');
    row.innerHTML = `
        <td>
         <input type="checkbox" name="Tasks[].IsCompleted" value="false" onclick="toggleCheckboxValue(this)" />
            <input type="hidden" name="Tasks[].Id" value="0" />
            <input type="hidden" name="Tasks[].Name" value="${taskName}" />
            <span class="truncate-text truncate-no-inline">${taskName}</span>
        </td>
        <td>
            <button type="button" class="btn btn-danger btn-sm" onclick="removeTask(this)">Delete</button>
        </td>
    `;

    taskContainer.appendChild(row);
    taskNameInput.value = '';

    reindexTasks();
}
