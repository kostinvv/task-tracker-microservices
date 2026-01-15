import { useSortable } from "@dnd-kit/sortable"
import { CSS } from "@dnd-kit/utilities";
import { EditOutlined } from '@ant-design/icons';
import { Card, Button, Flex } from 'antd';
import { DND_TASK_TYPE } from '../../../constants.js';

export function TaskItem({id, title, columnTitle, showUpdateTaskModal}) {
    const { attributes, listeners, setNodeRef, transform, transition, isDragging } = useSortable({
        id: id,
        data: {
            id: id,
            title: title,
            type: DND_TASK_TYPE
        }
    });

    const style = {
        transform: CSS.Transform.toString(transform),
        transition,
        opacity: isDragging ? 0 : 1
    };

    return (
        <div 
            ref={setNodeRef}
            style={style}
            {...attributes}
            {...listeners}
        >
            <Card
                hoverable
                size="small"
                style={{ 
                    marginBottom: 12,
                }}
            >
                <Flex justify="space-between" align="center">
                    <div style={{ fontSize: 16, fontWeight: 500, maxWidth: 200, textOverflow: 'ellipsis', whiteSpace: 'nowrap', overflow: 'hidden' }}>
                        { columnTitle === 'Done' ? <del>{title}</del> : title }
                    </div>
                    <Button onClick={async () => await showUpdateTaskModal(id)} type="default" icon={<EditOutlined />}></Button>
                </Flex>
            </Card>
        </div>
    )
}