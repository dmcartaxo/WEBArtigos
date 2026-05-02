import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import './Header.css';

const Header = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  if (!user) return null;

  return (
    <header className="header">
      <div className="header-container">
        <Link to="/" className="logo">
          WEBArtigos
        </Link>

        <nav className="nav-menu">
          <Link to="/" className="nav-link">
            Artigos
          </Link>
          <Link to="/create" className="nav-link">
            Novo Artigo
          </Link>
          <Link to="/upload" className="nav-link">
            Upload PDF
          </Link>
        </nav>

        <div className="user-section">
          <span className="user-email">{user.email}</span>
          {user.role === 'Admin' && <span className="admin-badge">Admin</span>}
          <button onClick={handleLogout} className="logout-btn">
            Sair
          </button>
        </div>
      </div>
    </header>
  );
};

export default Header;
