import { useDroppable } from "@dnd-kit/core";
import { rectSortingStrategy, SortableContext } from "@dnd-kit/sortable";
import { TaskItem } from "./TaskItem";
import { Card, Col } from 'antd';
import { DND_COLUMN_TYPE } from '../../../constants.js';

export function TaskColumn({ id, title, tasks }) {
    const { setNodeRef } = useDroppable({ 
        id: id,
        data: {
            type: DND_COLUMN_TYPE
        }
    });
    return (                
        <Col xs={24} sm={12} md={8}>
            <Card
                size="small" 
                title={title}
            >
                <div ref={setNodeRef}>
                    <SortableContext id={id} items={tasks.map((task => task.id))}> 
                        {tasks.map((task) => 
                            <TaskItem key={task.id} id={task.id} title={task.title} />
                        )} 
                    </SortableContext>      
                </div>     
            </Card>
        </Col>
    )
}