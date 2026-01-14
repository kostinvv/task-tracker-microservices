import { useDroppable } from "@dnd-kit/core";
import { rectSortingStrategy, SortableContext } from "@dnd-kit/sortable";
import { useContext } from 'react';
import { Context } from '../../../main.jsx';
import { TaskItem } from "./TaskItem";
import { Button, Card, Col } from 'antd';
import { DND_COLUMN_TYPE } from '../../../constants.js';

export function TaskColumn({ id, title, cursorList }) {
    const { store } = useContext(Context);
    const { setNodeRef } = useDroppable({ 
        id: id,
        data: {
            type: DND_COLUMN_TYPE
        }
    });

    const useStyles = (info) => {
        return {
            root: {
                borderColor: '#696FC7',
                borderRadius: 8,
            }
        }
    }

    const handleShowMore = () => {
        const stateId = id;
        const afterPosition = store.tasks.getLastPosition(stateId);
        store.tasks.loadMoreTasks(afterPosition, stateId);
    }

    return (                
        <Col xs={24} sm={12} md={8}>
            <Card
                size="small" 
                title={title}
                styles={useStyles}
            >
                <div ref={setNodeRef}>
                    <SortableContext id={id} items={cursorList.items.map((task => task.id))}> 
                        {cursorList.items.map((task) => 
                            <TaskItem key={task.id} id={task.id} title={task.title} />
                        )} 
                    </SortableContext>      
                </div>  
                { cursorList.hasNextPage ? <Button onClick={handleShowMore}>Show More</Button> : null }
            </Card>
        </Col>
    )
}