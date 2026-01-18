import { closestCorners, DndContext, PointerSensor, useSensor, useSensors, DragOverlay } from "@dnd-kit/core";
import { TaskItem } from "./TaskItem.jsx";
import { useContext, useEffect, useState } from 'react';
import { Context } from '../../../main.jsx';
import { observer } from 'mobx-react-lite';
import { TaskColumn } from "./TaskColumn";
import './TaskBoard.css';
import { Row, Modal, Button, Form, Input, Select } from 'antd';
import { DND_COLUMN_TYPE, DND_TASK_TYPE } from '../../../constants.js';

function TaskBoard() {
    const { store } = useContext(Context);

    const [addTaskForm] = Form.useForm();
    const [updateTaskForm] = Form.useForm();

    const [isCreateOpen, setIsCreateOpen] = useState(false);
    const [isDeleteOpen, setIsDeleteOpen] = useState(false);
    const [isUpdateOpen, setIsUpdateOpen] = useState(false);

    const [loading, setLoading] = useState(false);

    const [taskToUpdate, setTaskToUpdate] = useState(null);
    const [activeState, setActiveState] = useState(null);

    useEffect(() => {
        store.tasks.getBoard();
    }, []);

    useEffect(() => {
        if (!isUpdateOpen) {
            return;
        }

        updateTaskForm.setFieldsValue({
            title: taskToUpdate.title,
            description: taskToUpdate.description,
            state: String(taskToUpdate.state)
        });
    }, [isUpdateOpen, taskToUpdate, updateTaskForm]);

    useEffect(() => {
        if (!isCreateOpen) {
            return;
        }
        addTaskForm.setFieldsValue({
            state: activeState
        }); 
    }, [isCreateOpen, activeState, addTaskForm])

    const [activeData, setActiveData] = useState(null);
    const [prevActiveIndex, setPrevActiveIndex] = useState(null);

    const sensors = useSensors(
        useSensor(PointerSensor, {
            activationConstraint: { distance: 8 }
        })
    );

    const stateOptions = [
        { label: 'To Do', value: '0' },
        { label: 'In Progress', value: '1' },
        { label: 'Done', value: '2' }
    ];

    function handleDragStart(event) {
        const { active } = event;
        const { id } = active;

        const index = store.tasks.getTaskIndex(id);

        setPrevActiveIndex(index);
        setActiveData(active.data.current);
    }

    function handleDragOver(event) {
        const { active, over } = event;

        // Handle task sorting.
        if (active.data.current.type === DND_TASK_TYPE &&
            over?.data.current.type === DND_TASK_TYPE &&
            active && 
            over && 
            active.id !== over.id
        ) {
            store.tasks.reorderTask(active.id, over.id);
        }
        // Handling task drop into a column.
        if (active.data.current.type === DND_TASK_TYPE && 
            over?.data.current.type === DND_COLUMN_TYPE &&
            active && 
            over && 
            active.id !== over.id
        ) {
            store.tasks.moveTask(active.id, over.id);
        }
    }

    function handleDragEnd(event) {
        const { active } = event;
        store.tasks.saveTaskMove(prevActiveIndex, active.id);
        setPrevActiveIndex(null);
        setActiveData(null);
    }

    const showCreateTaskModal = (state) => {
        setActiveState(String(state));
        setIsCreateOpen(true);
    }

    const handleAddTask = async (values) => {
        setLoading(true);
        const { title, description, state } = values;
        await store.tasks.createTask({
            title,
            description,
            state: Number(state)
        });
        setLoading(false);
        setIsCreateOpen(false);
    }
    
    const handleAddTaskFail = (errorInfo) => { }

    const showUpdateTaskModal = async (id) => {
        setTaskToUpdate(await store.tasks.getTask(id));
        setIsUpdateOpen(true);
    };
    
    const handleUpdateTask = async (values) => {
        setLoading(true);
        const { id, sortOrder } = taskToUpdate;
        const { title, description, state } = values;

        const updateTaskRequest = {
            title,
            description,
            state: Number(state),
            sortOrder
        }

        await store.tasks.updateTask(id, taskToUpdate.state, updateTaskRequest);
        setLoading(false);
        setIsUpdateOpen(false);
    }

    const handleUpdateTaskFail = (errorInfo) => { }

    const handleDeleteTask = async () => {
        setLoading(true);
        await store.tasks.deleteTask(taskToUpdate.id, taskToUpdate.state);

        setLoading(false);
        setIsDeleteOpen(false);
        setIsUpdateOpen(false);
    }

    return (
        <>  
            <DndContext
                sensors={sensors}
                collisionDetection={closestCorners}
                onDragStart={handleDragStart}
                onDragOver={handleDragOver}
                onDragEnd={handleDragEnd}
            >
                <Row gutter={[16, 16]}>
                    {store.tasks.columns.map((column) =>
                        <TaskColumn 
                            key={column.id} 
                            id={column.id} 
                            title={column.title} 
                            pagedList={column.pagedList}
                            showCreateTaskModal={showCreateTaskModal}
                            showUpdateTaskModal={showUpdateTaskModal}
                        />
                    )}                
                </Row>
                <DragOverlay style={{ opacity: 0.5 }} dropAnimation={null}>
                    {activeData ? <TaskItem id={activeData.id} title={activeData.title} /> : null}
                </DragOverlay>
            </DndContext>

            <Modal
                title="Add Task"
                closable={{ 'aria-label': 'Close Button' }}
                open={isCreateOpen}
                onOk={() => addTaskForm.submit()}
                onCancel={() => setIsCreateOpen(false)}
                width={600}
                footer={[
                    <Button loading={loading} key="submit" type="primary" onClick={() => addTaskForm.submit()}>
                        Add
                    </Button>
                ]}
                afterClose={() => {
                    addTaskForm.resetFields();
                    setActiveState(null);
                }}
            >
                <Form
                    layout='vertical'
                    form={addTaskForm}
                    onFinish={handleAddTask}
                    onFinishFailed={handleAddTaskFail}
                >  
                    <Form.Item 
                        label="State" 
                        name="state"
                        rules={[{ required: true, message: 'Please enter State' }]}
                    >
                        <Select
                            placeholder="State"
                            options={stateOptions}
                        />
                    </Form.Item>
                    <Form.Item 
                        hasFeedback
                        validateTrigger="onBlur"
                        label="Title" 
                        name="title"
                        rules={[
                            { required: true, message: 'Please enter Title' }, 
                            { max: 256, message: 'Title must be up to 256 characters'}
                        ]}
                    >
                        <Input placeholder="Title" />
                    </Form.Item>
                    <Form.Item
                        label="Description"
                        name="description"
                        rules={[
                            { required: false }, 
                            { max: 1024, message: 'Description must be up to 1024 characters' }
                        ]}
                    >
                        <Input.TextArea rows={3} />
                    </Form.Item>
                </Form>
            </Modal>

            <Modal
                title="Delete Task"
                open={isDeleteOpen}
                onOk={handleDeleteTask}
                onCancel={() => setIsDeleteOpen(false)}
                cancelText="Cancel"
                footer={[
                    <Button loading={loading} key="submit" type="primary" danger onClick={handleDeleteTask}>Delete</Button>,
                ]}
            >
                <p>Confirm deletion</p>
            </Modal>

            <Modal
                title="Edit Task"
                closable={{ 'aria-label': 'Close Button' }}
                open={isUpdateOpen}
                onOk={() => updateTaskForm.submit()}
                onCancel={() => setIsUpdateOpen(false)}
                width={600}
                footer={[
                    <Button key="delete" type="text" danger onClick={() => setIsDeleteOpen(true)}>Delete</Button>,
                    <Button loading={loading} key="submit" type="primary" onClick={() => updateTaskForm.submit()}>
                        Save
                    </Button>
                ]}
                afterClose={() => {
                    updateTaskForm.resetFields();
                    setTaskToUpdate(null);
                }}
            >
                <Form
                    layout='vertical'
                    form={updateTaskForm}
                    onFinish={handleUpdateTask}
                    onFinishFailed={handleUpdateTaskFail}
                >  
                    <Form.Item 
                        label="State" 
                        name="state"
                        rules={[{ required: true, message: 'Please enter State' }]}
                    >
                        <Select
                            placeholder="State"
                            options={stateOptions}
                        />
                    </Form.Item>
                    <Form.Item 
                        label="Title" 
                        name="title"
                        rules={[
                            { required: true, message: 'Please enter Title' }, 
                            { max: 256, message: 'Title must be up to 256 characters'}
                        ]}
                    >
                        <Input placeholder="Title" />
                    </Form.Item>
                    <Form.Item 
                        label="Description" 
                        name="description"
                        rules={[
                            { required: false }, 
                            { max: 1024, message: 'Description must be up to 1024 characters' }
                        ]}
                    >
                        <Input.TextArea rows={3} />
                    </Form.Item>
                </Form>
            </Modal>
        </>
    )
}

export default observer(TaskBoard);
