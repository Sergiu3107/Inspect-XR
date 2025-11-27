package com.example.inspectxr.repository;

import com.example.inspectxr.entity.ModelEntity;
import com.example.inspectxr.entity.SessionEntity;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import java.util.List;
import java.util.Optional;

public interface SessionRepository extends JpaRepository<SessionEntity, Long> {

    @Query("SELECT s FROM SessionEntity s " +
            "LEFT JOIN FETCH s.participants " +
            "JOIN FETCH s.owner " +
            "JOIN FETCH s.model " +
            "WHERE s.id = :id")
    Optional<SessionEntity> findByIdWithAssociations(@Param("id") Long id);

    @Query("SELECT DISTINCT s FROM SessionEntity s " +
            "LEFT JOIN FETCH s.participants " +
            "JOIN FETCH s.owner " +
            "JOIN FETCH s.model")
    List<SessionEntity> findAllWithAssociations();

    @Query("SELECT DISTINCT s FROM SessionEntity s " +
            "LEFT JOIN FETCH s.participants " +
            "JOIN FETCH s.owner " +
            "JOIN FETCH s.model " +
            "WHERE s.owner.id = :ownerId")
    List<SessionEntity> findByOwnerIdWithAssociations(@Param("ownerId") Long ownerId);


}
