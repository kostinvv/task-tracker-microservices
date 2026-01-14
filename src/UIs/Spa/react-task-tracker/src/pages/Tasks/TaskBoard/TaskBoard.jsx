import { closestCorners, DndContext, PointerSensor, useSensor, useSensors, DragOverlay } from "@dnd-kit/core";
import { TaskItem } from "./TaskItem.jsx";
import { useMemo, useContext, useEffect, useState, useRef } from 'react';
import { Context } from '../../../main.jsx';
import { observer } from 'mobx-react-lite';
import { TaskColumn } from "./TaskColumn";
import './TaskBoard.css';
import { Row, Button, Flex, Input } from 'antd';
import { debounce } from "lodash";
import { DND_COLUMN_TYPE, DND_TASK_TYPE } from '../../../constants.js';
import { PlusOutlined } from '@ant-design/icons';

function TaskBoard() {
    const { store } = useContext(Context);

    useEffect(() => {
        store.tasks.getBoard();
    }, []);

    const [activeData, setActiveData] = useState(null);
    const [prevActiveIndex, setPrevActiveIndex] = useState(null);

    const lastMoveKeyRef = useRef(null);

    const sensors = useSensors(
        useSensor(PointerSensor, {
            activationConstraint: { distance: 8 }
        })
    );

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
        lastMoveKeyRef.current = null;
    }

    return (
        <>
            <Flex gap={"middle"} justify={'space-between'} style={{ marginBottom: 20 }}>
                <Button icon={<PlusOutlined />} type="primary">Create Task</Button>
                <Input.Search style={{ width: '20%' }} placeholder="Search" />
            </Flex>
            
            <DndContext
                sensors={sensors}
                collisionDetection={closestCorners}
                onDragStart={handleDragStart}
                onDragOver={handleDragOver}
                onDragEnd={handleDragEnd}
            >
                <Row gutter={[16, 16]}>
                    {store.tasks.columns.map((column) =>
                        <TaskColumn key={column.id} id={column.id} title={column.title} cursorList={column.cursorList} />
                    )}                
                </Row>
                <DragOverlay style={{ opacity: 0.5 }} dropAnimation={null}>
                    {activeData ? <TaskItem id={activeData.id} title={activeData.title} /> : null}
                </DragOverlay>
            </DndContext>
        </>
    )
}

export default observer(TaskBoard);
