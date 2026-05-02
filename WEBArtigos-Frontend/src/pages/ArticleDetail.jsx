import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { articleService } from '../services/api';
import { useAuth } from '../context/AuthContext';
import Alert from '../components/Alert';
import Loading from '../components/Loading';
import './ArticleDetail.css';

const ArticleDetail = () => {
  const { id } = useParams();
  const [article, setArticle] = useState(null);
  const [loading, setLoading] = useState(true);
  const [alert, setAlert] = useState(null);
  const { user } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    loadArticle();
  }, [id]);

  const loadArticle = async () => {
    try {
      const response = await articleService.getById(id);
      setArticle(response.data.data);
    } catch (error) {
      setAlert({
        type: 'error',
        message: 'Erro ao carregar artigo',
      });
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (!window.confirm('Tem certeza que deseja deletar este artigo?')) return;

    try {
      await articleService.delete(id);
      setAlert({
        type: 'success',
        message: 'Artigo deletado com sucesso!',
      });
      setTimeout(() => navigate('/'), 1500);
    } catch (error) {
      setAlert({
        type: 'error',
        message: error.response?.data?.errors?.[0] || 'Erro ao deletar artigo',
      });
    }
  };

  if (loading) {
    return <Loading />;
  }

  if (!article) {
    return (
      <div className="article-detail-container">
        <p>Artigo não encontrado</p>
      </div>
    );
  }

  return (
    <div className="article-detail-container">
      {alert && (
        <Alert
          type={alert.type}
          message={alert.message}
          onClose={() => setAlert(null)}
        />
      )}

      <article className="article-detail">
        <h1>{article.title}</h1>

        <div className="article-meta">
          <span className="author">Por: {article.author}</span>
          <span className="date">
            {new Date(article.createdAt).toLocaleDateString('pt-BR', {
              year: 'numeric',
              month: 'long',
              day: 'numeric',
            })}
          </span>
        </div>

        {article.summary && (
          <div className="article-summary-box">
            <h3>Resumo</h3>
            <p>{article.summary}</p>
          </div>
        )}

        {article.originalFileName && (
          <div className="article-source">
            <p>Arquivo original: {article.originalFileName}</p>
          </div>
        )}

        <div className="article-content">
          <p>{article.content}</p>
        </div>

        <div className="article-actions">
          <button onClick={() => navigate('/')} className="btn btn-back">
            Voltar
          </button>
          <button onClick={() => navigate(`/edit/${id}`)} className="btn btn-primary">
            Editar
          </button>
          {user?.role === 'Admin' && (
            <button onClick={handleDelete} className="btn btn-danger">
              Deletar
            </button>
          )}
        </div>
      </article>
    </div>
  );
};

export default ArticleDetail;
