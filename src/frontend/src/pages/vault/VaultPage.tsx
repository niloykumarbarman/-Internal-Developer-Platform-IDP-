import { useState } from 'react'
import { useMutation } from '@tanstack/react-query'
import { apiClient } from '../../api/client'
import { Shield, Key, RefreshCw, Eye, EyeOff, Plus, AlertTriangle, CheckCircle } from 'lucide-react'

const secrets = [
  { path: 'secret/idp/database', key: 'DB_PASSWORD', rotated: '7d ago', expires: '23d', status: 'Valid' },
  { path: 'secret/idp/github', key: 'GITHUB_TOKEN', rotated: '30d ago', expires: '0d', status: 'Expiring' },
  { path: 'secret/idp/jwt', key: 'JWT_SECRET', rotated: '1d ago', expires: '29d', status: 'Valid' },
  { path: 'secret/idp/redis', key: 'REDIS_PASSWORD', rotated: '14d ago', expires: '16d', status: 'Valid' },
  { path: 'secret/idp/sonar', key: 'SONAR_TOKEN', rotated: '60d ago', expires: '-5d', status: 'Expired' },
]

const statusColor: Record<string, string> = {
  Valid: 'oklch(0.6 0.18 150)', Expiring: 'oklch(0.75 0.18 80)', Expired: 'oklch(0.6 0.22 27)'
}

export default function VaultPage() {
  const [showValues, setShowValues] = useState<Record<string, boolean>>({})
  const [newPath, setNewPath] = useState('')
  const [newKey, setNewKey] = useState('')
  const [newVal, setNewVal] = useState('')

  const rotateMutation = useMutation({
    mutationFn: (path: string) => apiClient.post('/vault/rotate', { secretPath: path }),
  })

  const createMutation = useMutation({
    mutationFn: () => apiClient.post('/vault/secrets', { path: newPath, key: newKey, value: newVal }),
    onSuccess: () => { setNewPath(''); setNewKey(''); setNewVal('') },
  })

  const inp: React.CSSProperties = { width: '100%', padding: '0.55rem 0.75rem', background: 'oklch(0.22 0 0)', border: '1px solid oklch(0.32 0 0)', borderRadius: 6, color: 'oklch(0.95 0 0)', fontSize: 13, outline: 'none', boxSizing: 'border-box' }

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '1.25rem' }}>
      <div>
        <h1 style={{ fontSize: 22, fontWeight: 700, color: 'oklch(0.95 0 0)', marginBottom: 4 }}>Vault / Secrets Management</h1>
        <p style={{ fontSize: 13, color: 'oklch(0.5 0 0)' }}>HashiCorp Vault · Secret Rotation · K8s Auth</p>
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '0.75rem' }}>
        {[
          { label: 'Total Secrets', value: '47', color: 'oklch(0.6 0.2 250)' },
          { label: 'Valid', value: '44', color: 'oklch(0.6 0.18 150)' },
          { label: 'Expiring Soon', value: '2', color: 'oklch(0.75 0.18 80)' },
          { label: 'Expired', value: '1', color: 'oklch(0.6 0.22 27)' },
        ].map(s => (
          <div key={s.label} style={{ background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.28 0 0)', borderRadius: 8, padding: '1rem', textAlign: 'center' }}>
            <div style={{ fontSize: 26, fontWeight: 700, color: s.color }}>{s.value}</div>
            <div style={{ fontSize: 12, color: 'oklch(0.5 0 0)', marginTop: 4 }}>{s.label}</div>
          </div>
        ))}
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 320px', gap: '1rem' }}>
        {/* Secrets Table */}
        <div style={{ background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.28 0 0)', borderRadius: 10, overflow: 'hidden' }}>
          <div style={{ padding: '1rem 1.25rem', borderBottom: '1px solid oklch(0.25 0 0)', fontSize: 14, fontWeight: 600, color: 'oklch(0.85 0 0)', display: 'flex', alignItems: 'center', gap: 8 }}>
            <Key size={15} color="oklch(0.75 0.18 80)" /> Secret Inventory
          </div>
          <table style={{ width: '100%', borderCollapse: 'collapse' }}>
            <thead>
              <tr style={{ borderBottom: '1px solid oklch(0.25 0 0)' }}>
                {['Path', 'Key', 'Rotated', 'Expires', 'Status', 'Actions'].map(h => (
                  <th key={h} style={{ padding: '0.65rem 1rem', textAlign: 'left', fontSize: 11, fontWeight: 600, color: 'oklch(0.45 0 0)', textTransform: 'uppercase' }}>{h}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {secrets.map(s => (
                <tr key={s.key} style={{ borderBottom: '1px solid oklch(0.22 0 0)' }}>
                  <td style={{ padding: '0.75rem 1rem', fontSize: 12, color: 'oklch(0.6 0 0)', fontFamily: 'monospace' }}>{s.path}</td>
                  <td style={{ padding: '0.75rem 1rem' }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                      <span style={{ fontSize: 12, fontFamily: 'monospace', color: 'oklch(0.85 0 0)' }}>{s.key}</span>
                      <span style={{ fontSize: 11, fontFamily: 'monospace', color: 'oklch(0.4 0 0)' }}>
                        {showValues[s.key] ? '••••••••' : '••••••••'}
                      </span>
                      <button onClick={() => setShowValues(p => ({ ...p, [s.key]: !p[s.key] }))} style={{ background: 'none', border: 'none', cursor: 'pointer', color: 'oklch(0.5 0 0)', padding: 0 }}>
                        {showValues[s.key] ? <EyeOff size={13} /> : <Eye size={13} />}
                      </button>
                    </div>
                  </td>
                  <td style={{ padding: '0.75rem 1rem', fontSize: 12, color: 'oklch(0.55 0 0)' }}>{s.rotated}</td>
                  <td style={{ padding: '0.75rem 1rem', fontSize: 12, color: s.status === 'Expired' ? 'oklch(0.6 0.22 27)' : s.status === 'Expiring' ? 'oklch(0.75 0.18 80)' : 'oklch(0.55 0 0)' }}>{s.expires}</td>
                  <td style={{ padding: '0.75rem 1rem' }}>
                    <span style={{ fontSize: 11, fontWeight: 600, padding: '2px 8px', borderRadius: 20, color: statusColor[s.status], background: `${statusColor[s.status].replace(')', ' / 0.12)')}` }}>{s.status}</span>
                  </td>
                  <td style={{ padding: '0.75rem 1rem' }}>
                    <button onClick={() => rotateMutation.mutate(s.path)} style={{ display: 'flex', alignItems: 'center', gap: 4, padding: '3px 8px', background: 'oklch(0.25 0 0)', border: '1px solid oklch(0.35 0 0)', borderRadius: 4, color: 'oklch(0.7 0 0)', fontSize: 11, cursor: 'pointer' }}>
                      <RefreshCw size={11} /> Rotate
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {/* Add Secret */}
        <div style={{ background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.28 0 0)', borderRadius: 10, padding: '1.25rem', display: 'flex', flexDirection: 'column', gap: '0.85rem', height: 'fit-content' }}>
          <div style={{ fontSize: 14, fontWeight: 600, color: 'oklch(0.85 0 0)', display: 'flex', alignItems: 'center', gap: 8 }}>
            <Plus size={15} /> Add Secret
          </div>
          {[['Secret Path', newPath, setNewPath, 'secret/idp/myapp'], ['Key Name', newKey, setNewKey, 'MY_SECRET'], ['Value', newVal, setNewVal, 'secret-value']].map(([label, val, setter, ph]) => (
            <div key={label as string}>
              <label style={{ display: 'block', fontSize: 12, color: 'oklch(0.6 0 0)', marginBottom: 4 }}>{label as string}</label>
              <input style={inp} value={val as string} onChange={e => (setter as any)(e.target.value)} placeholder={ph as string} type={label === 'Value' ? 'password' : 'text'} />
            </div>
          ))}
          <button onClick={() => createMutation.mutate()} disabled={!newPath || !newKey || createMutation.isPending}
            style={{ padding: '0.6rem', background: 'oklch(0.6 0.2 250)', border: 'none', borderRadius: 6, color: '#fff', fontSize: 13, fontWeight: 600, cursor: 'pointer', display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 6 }}>
            <Shield size={14} /> Store Secret
          </button>
          {createMutation.isSuccess && <div style={{ fontSize: 12, color: 'oklch(0.6 0.18 150)', display: 'flex', alignItems: 'center', gap: 4 }}><CheckCircle size={13} /> Secret stored!</div>}
        </div>
      </div>
    </div>
  )
}
