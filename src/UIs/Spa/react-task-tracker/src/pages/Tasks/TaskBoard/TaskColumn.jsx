import { useDroppable } from "@dnd-kit/core";
import { SortableContext } from "@dnd-kit/sortable";
import { useContext } from 'react';
import { Context } from '../../../main.jsx';
import { TaskItem } from "./TaskItem";
import { Button, Card, Col } from 'antd';
import { DND_COLUMN_TYPE } from '../../../constants.js';
import { PlusOutlined } from '@ant-design/icons';

export function TaskColumn({ id, title, pagedList, showCreateTaskModal, showUpdateTaskModal }) {
    const { store } = useContext(Context);
    const { setNodeRef } = useDroppable({ 
        id: id,
        data: {
            type: DND_COLUMN_TYPE
        }
    });

    const handleShowMore = () => {
        const stateId = id;
        const skip = store.tasks.getLastPosition(stateId);
        store.tasks.loadMoreTasks(skip, stateId);
    }

    return (                
        <Col xs={24} sm={24} md={12} lg={12} xl={8}>
            <Card
                size="large" 
                title={title}
            >
                <div ref={setNodeRef}>
                    <SortableContext id={id} items={pagedList.items.map((task => task.id))}> 
                        {pagedList.items.map((task) => 
                            <TaskItem 
                                key={task.id} 
                                id={task.id} 
                                title={task.title} 
                                columnTitle={title}
                                showUpdateTaskModal={showUpdateTaskModal} 
                            />
                        )} 
                    </SortableContext>      
                </div>  
                { pagedList.hasNextPage ? <Button onClick={handleShowMore} style={{ marginBottom: 12 }}>Show More</Button> : null }
                <Button onClick={() => showCreateTaskModal(id)} type="dashed" size="large" icon={<PlusOutlined />} block>Add Task</Button>
            </Card>
        </Col>
    )
}