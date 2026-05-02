import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { authService } from '../services/api';
import Alert from '../components/Alert';
import './Login.css';

const Login = () => {
  const [email, setEmail] = useState('admin@webartigos.com');
  const [password, setPassword] = useState('Admin@123');
  const [loading, setLoading] = useState(false);
  const [alert, setAlert] = useState(null);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);

    try {
      const response = await authService.login(email, password);
      const { data } = response.data;

      login(
        { email: data.email, role: data.role },
        data.token
      );

      setAlert({ type: 'success', message: 'Login realizado com sucesso!' });
      setTimeout(() => navigate('/'), 1500);
    } catch (error) {
      setAlert({
        type: 'error',
        message: error.response?.data?.errors?.[0] || 'Erro ao fazer login',
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-container">
      {alert && (
        <Alert
          type={alert.type}
          message={alert.message}
          onClose={() => setAlert(null)}
        />
      )}

      <div className="login-card">
        <h1>WEBArtigos</h1>
        <p>Gerenciador de Artigos</p>

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="email">Email</label>
            <input
              type="email"
              id="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              disabled={loading}
            />
          </div>

          <div className="form-group">
            <label htmlFor="password">Senha</label>
            <input
              type="password"
              id="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              disabled={loading}
            />
          </div>

          <button type="submit" disabled={loading}>
            {loading ? 'Entrando...' : 'Entrar'}
          </button>
        </form>

        <div className="demo-info">
          <p>Credenciais de demonstração:</p>
          <p>Email: admin@webartigos.com</p>
          <p>Senha: Admin@123</p>
        </div>
      </div>
    </div>
  );
};

export default Login;
