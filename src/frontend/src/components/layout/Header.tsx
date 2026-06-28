import { Bell, Search, ChevronDown, LogOut, User, Settings } from 'lucide-react'
import { useState } from 'react'
import { useAuthStore } from '../../store/authStore'
import { useNavigate, useLocation } from 'react-router-dom'

const routeLabels: Record<string, string> = {
  '/dashboard': 'Platform Dashboard',
  '/catalog': 'Service Catalog',
  '/self-service': 'Self Service',
  '/gitops': 'GitOps',
  '/cicd': 'CI/CD Pipelines',
  '/kubernetes': 'Kubernetes',
  '/infrastructure': 'Infrastructure',
  '/observability': 'Observability',
  '/devsecops': 'DevSecOps',
  '/vault': 'Vault & Secrets',
  '/cost': 'Cost Management',
  '/incidents': 'Incidents',
  '/audit': 'Audit Logs',
  '/teams': 'Team Management',
}

export default function Header() {
  const [showMenu, setShowMenu] = useState(false)
  const { user, logout } = useAuthStore()
  const navigate = useNavigate()
  const location = useLocation()
  const pageTitle = routeLabels[location.pathname] ?? 'Enterprise IDP'

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  return (
    <header className="h-14 bg-[#0d1526]/80 backdrop-blur border-b border-white/5 flex items-center px-6 gap-4 flex-shrink-0 z-10">
      {/* Page Title */}
      <div className="text-slate-300 text-sm font-medium">{pageTitle}</div>

      {/* Search */}
      <div className="flex-1 max-w-md mx-auto">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-3.5 h-3.5 text-slate-500" />
          <input
            placeholder="Search services, pipelines..."
            className="w-full bg-white/5 border border-white/8 rounded-lg pl-9 pr-4 py-1.5 text-sm text-slate-300 placeholder-slate-600 focus:outline-none focus:border-blue-500/50 focus:bg-white/8 transition-all"
          />
          <kbd className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-600 text-xs bg-white/5 px-1.5 py-0.5 rounded border border-white/8">⌘K</kbd>
        </div>
      </div>

      <div className="flex items-center gap-2 ml-auto">
        {/* Notification */}
        <button className="relative w-8 h-8 flex items-center justify-center rounded-lg hover:bg-white/8 text-slate-400 hover:text-white transition-all">
          <Bell className="w-4 h-4" />
          <span className="absolute top-1 right-1 w-2 h-2 bg-red-500 rounded-full border border-[#0d1526]" />
        </button>

        {/* User Menu */}
        <div className="relative">
          <button
            onClick={() => setShowMenu(!showMenu)}
            className="flex items-center gap-2 pl-2 pr-3 py-1.5 rounded-lg hover:bg-white/8 transition-all group"
          >
            <div className="w-7 h-7 rounded-lg bg-gradient-to-br from-blue-500 to-violet-600 flex items-center justify-center text-white text-xs font-bold shadow-lg">
              {user?.name?.charAt(0) ?? 'U'}
            </div>
            <div className="text-left hidden sm:block">
              <p className="text-white text-xs font-medium leading-none">{user?.name ?? 'User'}</p>
              <p className="text-slate-500 text-xs leading-none mt-0.5">{user?.role ?? 'Admin'}</p>
            </div>
            <ChevronDown className="w-3 h-3 text-slate-500 group-hover:text-slate-300 transition-colors" />
          </button>

          {showMenu && (
            <>
              <div className="fixed inset-0 z-10" onClick={() => setShowMenu(false)} />
              <div className="absolute right-0 top-full mt-1 w-48 bg-[#111827] border border-white/10 rounded-xl shadow-2xl z-20 overflow-hidden py-1">
                <div className="px-3 py-2 border-b border-white/5 mb-1">
                  <p className="text-white text-xs font-medium">{user?.email}</p>
                </div>
                {[
                  { icon: User, label: 'Profile' },
                  { icon: Settings, label: 'Settings' },
                ].map(item => (
                  <button key={item.label} className="w-full flex items-center gap-2.5 px-3 py-2 text-slate-400 hover:text-white hover:bg-white/5 text-sm transition-all">
                    <item.icon className="w-3.5 h-3.5" />
                    {item.label}
                  </button>
                ))}
                <div className="border-t border-white/5 mt-1 pt-1">
                  <button
                    onClick={handleLogout}
                    className="w-full flex items-center gap-2.5 px-3 py-2 text-red-400 hover:text-red-300 hover:bg-red-500/10 text-sm transition-all"
                  >
                    <LogOut className="w-3.5 h-3.5" />
                    Sign Out
                  </button>
                </div>
              </div>
            </>
          )}
        </div>
      </div>
    </header>
  )
}
