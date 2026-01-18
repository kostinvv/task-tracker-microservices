import './App.css';
import { useContext, useEffect, useState } from 'react';
import { Context } from './main.jsx';
import { observer } from 'mobx-react-lite';
import { NavLink, useNavigate } from 'react-router-dom';
import { MenuFoldOutlined, MenuUnfoldOutlined, UserOutlined, LoginOutlined, LogoutOutlined, CheckCircleOutlined } from '@ant-design/icons';
import { Button, Layout, Menu, theme } from 'antd';
import { AppRouters } from './AppRouters.jsx';

const { Header, Sider, Content } = Layout;

function App() {
  const [collapsed, setCollapsed] = useState(false);
  const {
    token: { colorBgContainer, borderRadiusLG },
  } = theme.useToken();

  const { store } = useContext(Context);
  const navigate = useNavigate();

  useEffect(() => {
    async function checkAuthOnLoad() {
      if (localStorage.getItem('accessToken')) {
        await store.auth.checkAuth();
        if (store.auth.isAuth) {
          navigate('/tasks');
        } else {
          navigate('/login');
        }
      } else if (window.location.pathname === '/') {
        navigate('/login');
      }
    }
    checkAuthOnLoad();
  }, []);

  const menuItems = !store.auth.isAuth ?[
    {
      key: '/login',
      icon: <LoginOutlined />,
      label: <NavLink to="/login">Log In</NavLink>,
    },
    {
      key: '/signup',
      icon: <UserOutlined />,
      label: <NavLink to="/signup">Sign Up</NavLink>,
    }
  ] : [
    {
      key: '/tasks',
      icon: <CheckCircleOutlined />,
      label: <NavLink to="/tasks">Tasks</NavLink>,
    },
    {
      key: 'logout',
      icon: <LogoutOutlined />,
      label: <a onClick={() => {
        store.auth.logOut();
        navigate('/login');
      }}>Log Out</a>,
    }
  ];

  const siderStyle = {
    overflow: 'auto',
    height: '100vh',
    position: 'sticky',
    insetInlineStart: 0,
    top: 0,
    scrollbarWidth: 'thin',
    scrollbarGutter: 'stable',
  };

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Sider trigger={null} collapsible collapsed={collapsed} style={siderStyle}>
        <Menu
          theme="dark"
          mode="inline"
          selectedKeys={[location.pathname]}
          items={menuItems}
        />
      </Sider>
      <Layout>
        <Header style={{ padding: 0, background: colorBgContainer, position: 'sticky', top: 0, zIndex: 1, width: '100%' }}>
          <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
            <Button
              type="text"
              icon={collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
              onClick={() => setCollapsed(!collapsed)}
              style={{
                fontSize: '16px',
                width: 64,
                height: 64,
              }}
            />
            <div style={{ padding: '0 16px' }}>{store.auth.user.email}</div>
          </div>
        </Header>
        <Content
          style={{
            padding: 16,
            background: '#B6D0E2',
            overflow: 'hidden',
          }}
        >
          <AppRouters/>
        </Content>
      </Layout>
    </Layout>
  ) 
}

export default observer(App);
