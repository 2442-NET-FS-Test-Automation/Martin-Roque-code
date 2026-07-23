import './App.css'
import { CatalogPage } from './pages/CatalogPage';
import { BookDetail } from './pages/BookDetail';
import { About } from './pages/About';
import { LoginPage } from './pages/LoginPage';
import { NavLink, Route, Routes } from 'react-router-dom';
import { RequireAuth } from './components/RequireAuth';
import { AdminPage } from './pages/AdminPage';
import { useAuth } from './auth/useAuth';


function App() {

  const { status, user, logout} = useAuth();

  return (
    <>
      <div className='app'></div>
      <header className='app-header'>
        <h1>Library</h1>
        <nav className='app-header'>
          <NavLink to="/">Catalog</NavLink>
          <NavLink to="/about">About</NavLink>
          {user?.role === "admin" && <NavLink to="/admin">Admin</NavLink>}
        </nav>
        <div className='auth-box'>
          {status === "authenticated" ? (
            <>
              <span>
                {user?.name} ({user?.role})
              </span>
              <button type="button" onClick={logout}>
                Sign out
              </button>
            </>
          ) : (
            <NavLink to="/login">Sign in</NavLink>
          )}
        </div>
      </header>

        <main>
          <Routes>
            <Route path='/' element={<CatalogPage />}/>
            <Route path='/inventory/:sku' element={<BookDetail />} />
            <Route path='/about' element={<About />}/>
            <Route path='/login' element={<LoginPage/>}/>
            <Route path='/admin' element={
              <RequireAuth role="admin">
                <AdminPage />
              </RequireAuth>
            }/>
            <Route path='*' element={<p>Page not found</p>}/>
          </Routes>
        </main>
    </>
  );
}

export default App
