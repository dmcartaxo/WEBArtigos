import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { articleService } from '../services/api';
import { useAuth } from '../context/AuthContext';
import Alert from '../components/Alert';
import Loading from '../components/Loading';
import './ArticlesList.css';

const ArticlesList = () => {
  const [articles, setArticles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchQuery, setSearchQuery] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(1);
  const [alert, setAlert] = useState(null);
  const { user } = useAuth();

  useEffect(() => {
    loadArticles();
  }, [currentPage]);

  const loadArticles = async () => {
    setLoading(true);
    try {
      const response = await articleService.getAll(currentPage, pageSize);
      const { items, totalCount } = response.data.data;
      setArticles(items);
      setTotalPages(Math.ceil(totalCount / pageSize));
    } catch (error) {
      setAlert({
        type: 'error',
        message: 'Erro ao carregar artigos',
      });
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = async (e) => {
    e.preventDefault();
    if (!searchQuery.trim()) {
      loadArticles();
      return;
    }

    setLoading(true);
    try {
      const response = await articleService.search(searchQuery, 1, pageSize);
      const { items, totalCount } = response.data.data;
      setArticles(items);
      setTotalPages(Math.ceil(totalCount / pageSize));
      setCurrentPage(1);
    } catch (error) {
      setAlert({
        type: 'error',
        message: 'Erro ao buscar artigos',
      });
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Tem certeza que deseja deletar este artigo?')) return;

    try {
      await articleService.delete(id);
      setAlert({
        type: 'success',
        message: 'Artigo deletado com sucesso!',
      });
      loadArticles();
    } catch (error) {
      setAlert({
        type: 'error',
        message: error.response?.data?.errors?.[0] || 'Erro ao deletar artigo',
      });
    }
  };

  if (loading && articles.length === 0) {
    return <Loading />;
  }

  return (
    <div className="articles-container">
      {alert && (
        <Alert
          type={alert.type}
          message={alert.message}
          onClose={() => setAlert(null)}
        />
      )}

      <div className="articles-header">
        <h1>Artigos</h1>
        <form onSubmit={handleSearch} className="search-form">
          <input
            type="text"
            placeholder="Buscar artigos..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
          />
          <button type="submit">Buscar</button>
        </form>
      </div>

      {loading ? (
        <div className="loading-spinner">Carregando...</div>
      ) : articles.length === 0 ? (
        <div className="no-articles">
          <p>Nenhum artigo encontrado</p>
        </div>
      ) : (
        <>
          <div className="articles-grid">
            {articles.map((article) => (
              <article key={article.id} className="article-card">
                <div className="article-header">
                  <h2>
                    <Link to={`/article/${article.id}`}>{article.title}</Link>
                  </h2>
                  <span className="article-date">
                    {new Date(article.createdAt).toLocaleDateString('pt-BR')}
                  </span>
                </div>

                <p className="article-author">Por: {article.author}</p>

                {article.summary && (
                  <p className="article-summary">{article.summary.substring(0, 150)}...</p>
                )}

                <div className="article-actions">
                  <Link to={`/article/${article.id}`} className="btn btn-primary">
                    Ver Completo
                  </Link>
                  <Link to={`/edit/${article.id}`} className="btn btn-secondary">
                    Editar
                  </Link>
                  {user?.role === 'Admin' && (
                    <button
                      onClick={() => handleDelete(article.id)}
                      className="btn btn-danger"
                    >
                      Deletar
                    </button>
                  )}
                </div>
              </article>
            ))}
          </div>

          {totalPages > 1 && (
            <div className="pagination">
              <button
                onClick={() => setCurrentPage(Math.max(1, currentPage - 1))}
                disabled={currentPage === 1}
              >
                Anterior
              </button>
              <span>
                Página {currentPage} de {totalPages}
              </span>
              <button
                onClick={() => setCurrentPage(Math.min(totalPages, currentPage + 1))}
                disabled={currentPage === totalPages}
              >
                Próxima
              </button>
            </div>
          )}
        </>
      )}
    </div>
  );
};

export default ArticlesList;
