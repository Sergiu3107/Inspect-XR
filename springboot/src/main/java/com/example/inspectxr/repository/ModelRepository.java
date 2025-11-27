package com.example.inspectxr.repository;

import com.example.inspectxr.entity.ModelEntity;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;

@Repository
public interface ModelRepository extends JpaRepository<ModelEntity, Long> {

    Optional<ModelEntity> findById(Long id);
    List<ModelEntity> findByOwnerId(Long ownerId);
}
