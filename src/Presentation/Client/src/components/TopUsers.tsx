import {useEffect, useState} from 'react';
import {Bar, BarChart, CartesianGrid, Legend, ResponsiveContainer, Tooltip, XAxis, YAxis} from 'recharts';
import {jiraApi} from '../services/api';
import type {TopUsersDto} from '../types/api';

interface TopUsersProps {
  projectKey: string;
}


export function TopUsers({ projectKey }: TopUsersProps) {
  const [data, setData] = useState<TopUsersDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const response = await jiraApi.getTopUsers(projectKey);
        setData(response);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Ошибка загрузки данных');
      } finally {
        setLoading(false);
      }
    };

    if (projectKey) {
      fetchData();
    }
  }, [projectKey]);

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-gray-500">Загрузка...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-red-500">Ошибка: {error}</div>
      </div>
    );
  }

  if (!data || !data.topUsers || data.topUsers.length === 0) {
    return (
      <div className="w-full h-96">
        <h3 className="text-xl font-semibold mb-4 text-gray-800">Топ-30 пользователей</h3>
        <div className="flex items-center justify-center h-full">
          <div className="text-gray-500">Нет данных для отображения</div>
        </div>
      </div>
    );
  }

  const usersData = data.topUsers.map(item => ({
    name: item.userName,
    total: item.totalCount,
    reporter: item.reporterCount,
    assignee: item.assigneeCount,
  }));

  return (
    <div className="w-full">
      <h3 className="text-xl font-semibold mb-4 text-gray-800">
        Топ-30 пользователей по общему количеству задач
      </h3>
      <p className="text-sm text-gray-600 mb-4">
        График показывает пользователей, у которых суммарно больше всего задач (как репортер + как исполнитель).
        Зеленым показаны задачи, где пользователь — репортер, синим — где исполнитель.
      </p>
      <div className="h-96">
        <ResponsiveContainer width="100%" height="100%">
          <BarChart data={usersData} layout="vertical" margin={{ top: 20, right: 30, left: 20, bottom: 20 }}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis type="number" label={{ value: 'Количество задач', position: 'insideBottom', offset: -5 }} />
            <YAxis 
              dataKey="name" 
              type="category" 
              width={200}
              tick={{ fontSize: 12 }}
            />
            <Tooltip 
              content={({ active, payload }) => {
                if (active && payload && payload.length) {
                  const data = payload[0].payload;
                  return (
                    <div className="bg-white p-3 border border-gray-300 rounded shadow-md">
                      <p className="font-semibold">{data.name}</p>
                      <p className="text-sm text-blue-600">Репортер: {data.reporter} задач</p>
                      <p className="text-sm text-green-600">Исполнитель: {data.assignee} задач</p>
                      <p className="text-sm font-semibold">Всего: {data.total} задач</p>
                    </div>
                  );
                }
                return null;
              }}
            />
            <Legend />
            <Bar dataKey="reporter" stackId="a" fill="#10b981" name="Репортер" />
            <Bar dataKey="assignee" stackId="a" fill="#3b82f6" name="Исполнитель" />
          </BarChart>
        </ResponsiveContainer>
      </div>
    </div>
  );
}

