import { useContext, useState } from "react"
import { Context } from '../main.jsx';
import { observer } from "mobx-react-lite";
import { Button, Form, Input, Alert, Card, message } from "antd";
import { useNavigate } from "react-router-dom";

function LogIn() {
    const { store } = useContext(Context);
    const [errorMessage, setErrorMessage] = useState(null);
    const [messageApi, contextHolder] = message.useMessage();
    const key = 'loaded';

    const navigate = useNavigate();

    async function onFinish (values) {
        const { email, password } = values;

        messageApi.open({
            key,
            type: 'loading',
            content: 'Loading...',
            duration: 0
        });

        const errorMessage = await store.auth.logIn(email, password);
        messageApi.destroy(key);
        
        if (errorMessage) {
            setErrorMessage(errorMessage);
        } else {
            navigate('/tasks');
        }
    }

    function onFinishFailed (errorInfo) {
        setErrorMessage(null);
    }

    return (
        <>
            {contextHolder}
            <Card style={{ maxWidth: 800 }}>
                <Form
                    name="basic"
                    layout='vertical'
                    style={{ maxWidth: 800 }}
                    onFinish={onFinish}
                    onFinishFailed={onFinishFailed}
                    autoComplete="off"
                >
                    <h1>Log In</h1>
                    { errorMessage ? <Alert style={{ marginBottom: 16 }} type="error" title={errorMessage} banner /> : null }
                    <Form.Item
                        label='E-mail'
                        name='email'
                        rules={[{ required: true, message: 'Please input your e-mail' }]}
                    >
                        <Input placeholder="Enter your e-mail" />
                    </Form.Item>
                    <Form.Item
                        label='Password'
                        name='password'
                        rules={[{ required: true, message: 'Please input your password' }]}
                    >
                        <Input.Password placeholder="Enter your password" />
                    </Form.Item>
                    <Form.Item label={null}>
                        <Button type="primary" htmlType="submit">
                            Log In
                        </Button>
                    </Form.Item>
                </Form>
            </Card>
        </>
    )
}

export default observer(LogIn);